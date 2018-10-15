using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class Player : NetworkBehaviour {
    public static Player LocalPlayer;

    public GameObject firstPersonCharacter;
    public GameObject PlayerModel;
    public GameObject playerGUI;
    public Text TextPlayerName;
    

    [SyncVar]
    public string playerName;

    [SyncVar (hook = "OnLaughCountChanged")]
    public int laughCount;

    private Animator anim;

    private bool jokeInProgress;

    private bool isMoving;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        TextPlayerName.text = playerName;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        playerGUI.SetActive(false);
        LocalPlayer = this;
        
    }

    // Update is called once per frame
    void Update () {
        if (isLocalPlayer)
        {
            bool isCurrentlyMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
            if (!isMoving && isCurrentlyMoving)
            {
                isMoving = true;
                anim.SetInteger("state", 1);
            }
            else if(isMoving && !isCurrentlyMoving)
            {
                isMoving = false;
                anim.SetInteger("state", 0);
            }
        }
	}

    public void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerModel.SetActive(false);
            firstPersonCharacter.SetActive(true);
            var controller = GetComponent<FirstPersonController>();
            controller.enabled = true;
        }
    }

    public void DisablePlayer()
    {
        if(isLocalPlayer)
        {
            var controller = GetComponent<FirstPersonController>();
            controller.enabled = false;
            firstPersonCharacter.SetActive(false);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (jokeInProgress) return;

        if (other.tag == "Enemy")
        {
            jokeInProgress = true;

            other.gameObject.SetActive(false);

            CmdFoundLaugh();

            Joke joke = UIManager.instance.GetRandomJoke();
            
        }

        if(other.tag == "Guards")
        {
            if(laughCount >= UIManager.instance.TotalLaughs)
            {
                UIManager.instance.AccessGranted();
            }
            else
            {
                UIManager.instance.AccessDenied();
            }
        }
    }

    public void JokeComplete()
    {
        jokeInProgress = false;
    }

    [Command]
    public void CmdFoundLaugh()
    {
        ++laughCount;
    }

    [Command]
    public void CmdWin()
    {
        UIManager.instance.winnerId = (int)netId.Value; // This will never work on the server because player will not exist
        print("CmdWin: " + UIManager.instance.winnerId);
    }

    public void OnLaughCountChanged(int count)
    {
        if (!isLocalPlayer) return;

        laughCount = count;
        UIManager.instance.UpdateLaughCount(count);
    }
}
