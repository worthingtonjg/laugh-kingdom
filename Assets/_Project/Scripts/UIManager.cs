using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Fungus;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class UIManager : NetworkBehaviour {
    public static UIManager instance;

    public Image LaughBarImage;
    public Text LaughBarText;
    public GameObject JokesFlowChart;
    public GameObject CutsceneFlowChart;
    public List<Joke> Jokes;
    public int TotalJokeCount = 16;
    public int TotalLaughs = 16;
    public GameObject CutsceneCamera;

    [SyncVar (hook = "OnWinnerChanged")]
    public int winnerId;
    
    private Flowchart FlowchartScript;
    private Flowchart CutsceneFlowchartScript;
    private List<Player> players;
    private NetworkLobbyManager lobbyManagerScript;

    // Use this for initialization
    void Start () {
        instance = this;
        UpdateLaughCount(0);
        FlowchartScript = JokesFlowChart.GetComponent<Flowchart>();
        CutsceneFlowchartScript = CutsceneFlowChart.GetComponent<Flowchart>();
        Jokes = new List<Joke>();
        // Initalize the list of jokes
        for(int i = 1; i <= TotalJokeCount; i++)
        {
            Jokes.Add(new Joke { Name = "Joke" + i });
        }

        // Get reference to the lobbyHookscript so can get players
        var lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager");

        if (lobbyManager != null)
        {
            var lobbyHookScript = lobbyManager.GetComponent<NetworkLobbyHook>();
            lobbyManagerScript = lobbyManager.GetComponent<NetworkLobbyManager>();

            players = lobbyHookScript.players;
        }
	}

    public void UpdateLaughCount(int current)
    {
        LaughBarImage.fillAmount = (float)current / (float)TotalLaughs;
        LaughBarText.text = current + " / " + TotalLaughs;
    }

    public Joke GetRandomJoke()
    {
        Joke result = null;

        var unused = Jokes.Where(j => !j.Found).ToList();

        var randomIndex = Random.Range(0, unused.Count);

        result = unused[randomIndex];
        result.Found = true;

        FlowchartScript.ExecuteBlock(result.Name);

        print(result.Name);

        return result;
    }

    public void JokeComplete()
    {
        Player.LocalPlayer.JokeComplete();
        print("joke complete");
    }

    public void StartGame()
    {
        Camera.main.gameObject.SetActive(false);
        Player.LocalPlayer.EnablePlayer();
    }

    public void AccessDenied()
    {
        CutsceneFlowchartScript.ExecuteBlock("AccessDenied");
    }

    public void AccessGranted()
    {
        // Check if anybody else has already won
        if (winnerId > 0) return;

        // Make the player issue a command to the server telling the server they have won
        Player.LocalPlayer.CmdWin();
    }

    // This runs on the client when it is notified that it has won
    public void OnWinnerChanged(int id)
    {
        winnerId = id;
        print("OnWinnerChanged: " + winnerId);

        CutsceneFlowchartScript.ExecuteBlock("Fadeout");

        StartCoroutine(EndingCutScene());
    }

    private IEnumerator EndingCutScene()
    {
        yield return new WaitForSeconds(2);

        Player.LocalPlayer.DisablePlayer();
        CutsceneCamera.SetActive(true);

        if (winnerId != (int)Player.LocalPlayer.netId.Value)
        {
            CutsceneFlowchartScript.ExecuteBlock("EndGameLose");
        }
        else
        {
            CutsceneFlowchartScript.ExecuteBlock("EndGameWin");
        }
    }

    public void BackToLobby()
    {
        Cursor.visible = true;
        ((LobbyManager)lobbyManagerScript).GoBackButton();
    }
}
