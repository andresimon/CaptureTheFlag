using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerController101 player = gamePlayer.GetComponent<PlayerController101>();

        if ( lobby.playerName.Length > 0)
            player.playerName = lobby.playerName;
        else
            player.playerName = "Player";

        player.color = lobby.playerColor;
      //  spaceship.score = 0;
      //  spaceship.lifeCount = 3;
    }
}
