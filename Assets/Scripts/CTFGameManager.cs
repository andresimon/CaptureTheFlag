using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CTFGameManager : NetworkBehaviour {

    public int m_numPlayers = 2;
    public float m_gameTime = 60.0f;

    public GameObject m_flag = null;

    public enum CTF_GameState
    {
        GS_WaitingForPlayers,
        GS_Ready,
        GS_InGame,
        GS_GameOver,
    }

    static int playerNum = 0;

    int ps1Score, ps2Score, ps3Score, ps4Score;
    string[] psNames;

    [SyncVar]
    CTF_GameState m_gameState = CTF_GameState.GS_WaitingForPlayers;

    public bool SpawnFlag()
    {
       //original
        Vector3 spawnPoint;
        ObjectSpawner.RandomPoint(new Vector3(0, 0, 0), 10.0f, out spawnPoint);
        GameObject flag = Instantiate(m_flag, spawnPoint, new Quaternion());

     //   GameObject flag = Instantiate(m_flag, new Vector3(0, 0, 0), new Quaternion());
        NetworkServer.Spawn(flag);
        return true;
    }

    bool IsNumPlayersReached()
    {
        return CTFNetworkManager.singleton.numPlayers == m_numPlayers;
    }

	// Use this for initialization
	void Start () 
    {
       psNames = new string[4]; 
            
    }
	
	// Update is called once per frame
	void Update ()
    {
        if ( isServer && m_gameState == CTF_GameState.GS_InGame )
        {
            m_gameTime -= Time.deltaTime;

           if (m_gameTime <= 0.0f)
               m_gameState = CTF_GameState.GS_GameOver;
        }
            
	    if(isServer)
        {
            if(m_gameState == CTF_GameState.GS_WaitingForPlayers && IsNumPlayersReached())
            {
                m_gameState = CTF_GameState.GS_Ready;

            }
        }

        PlayerController101[] players = GameObject.FindObjectsOfType <PlayerController101>();
        for ( int x = 0; x < players.Length; x++)
        {
            if ( players[x].HasFlag() )
            {
                if ( players[x].playerNum == 1)
                    ps1Score++;
                else if ( players[x].playerNum == 2)
                    ps2Score++;
                else if ( players[x].playerNum == 3)
                    ps3Score++;
                else if ( players[x].playerNum == 4)
                    ps4Score++;
 
                break;
            }
        }

        UpdateGameState();

        HUD.instance.DisplayTimer(m_gameTime);
        HUD.instance.DisplayScore(psNames, ps1Score, ps2Score, ps3Score, ps4Score);
	}

    public void UpdateGameState()
    {
        if (m_gameState == CTF_GameState.GS_Ready)
        {
            //call whatever needs to be called
            if (isServer)
            {
                SpawnFlag();
                //change state to ingame
                m_gameState = CTF_GameState.GS_InGame;

                PlayerController101[] players = GameObject.FindObjectsOfType <PlayerController101>();
                for (int x = 0; x < players.Length; x++)
                {
                    psNames[x] = players[x].playerName;
                }
            }
        }

        if ( isServer && m_gameState == CTF_GameState.GS_GameOver )
        {
            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.ServerChangeScene("Offline");
        }
        
    }

    static public int AssignPlayerNum()
    {
        playerNum++;
        return playerNum;
    }
}
