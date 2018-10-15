using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
    public List<Player> players;

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        print("*** OnLobbyServerSceneLoadedForPlayer");

        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);

        var lobbyPlayerScript = lobbyPlayer.GetComponent<LobbyPlayer>();
        var playerScript = gamePlayer.GetComponent<Player>();

        playerScript.playerName = lobbyPlayerScript.playerName;

        players.Add(gamePlayer.GetComponent<Player>());

        print("*** Player = " + gamePlayer.name);
    }
}
