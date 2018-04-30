using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HUD : NetworkBehaviour 
{
    public static HUD instance;

    Text player1_Score;
    Text player2_Score;
    Text player3_Score;
    Text player4_Score;

    string player1_Name;
    string player2_Name;
    string player3_Name;
    string player4_Name;

    Text timer;

    void Awake () 
    {
        if(instance == null)
        {
            instance = this;

            player1_Score = transform.Find("Player1_Score").GetComponent<Text>();
            player2_Score = transform.Find("Player2_Score").GetComponent<Text>();
            player3_Score = transform.Find("Player3_Score").GetComponent<Text>();
            player4_Score = transform.Find("Player4_Score").GetComponent<Text>();

            timer = transform.Find("Timer").GetComponent<Text>();

            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }


    }

    void Start()
    {
       
    }

    public void DisplayScore(string[] psNames, int p1Score, int p2Score, int p3Score, int p4Score)
    {
        player1_Score.text = string.Format("Player1: {0}", p1Score);
       /*
        if ( psNames[0] != "")
            player1_Score.text = psNames[0] + ": " + p1Score;

        if ( psNames[1] != "")
            player2_Score.text = psNames[1] + ": " + p2Score;
        */
        player2_Score.text = string.Format("Player2: {0}", p2Score);
        player3_Score.text = string.Format("Player3: {0}", p3Score);
        player4_Score.text = string.Format("Player4: {0}", p4Score);



    }

    public void SetupHUD()
    {
        if (!isServer) { return;}

        player2_Score.enabled = false;
        player3_Score.enabled = false;
        player4_Score.enabled = false;

        PlayerController101[] players = GameObject.FindObjectsOfType <PlayerController101>();

        player1_Score.color = players[0].color;
        player1_Name = players[0].playerName;

        if ( players.Length >=2 )
        {
            player2_Score.enabled = true;
            player2_Score.color = players[1].color;
            player2_Name = players[1].playerName;
        }
        if ( players.Length >=3 )
        {
            player3_Score.enabled = true;
            player3_Score.color = players[2].color;
            player3_Name = players[2].playerName;
        }
        if ( players.Length >=4 )
        {
            player4_Score.enabled = true;
            player4_Score.color = players[3].color;
            player4_Name = players[3].playerName;  
        }
    }

    public void DisplayTimer(float gameTime)
    {
        timer.text = string.Format( "{0}", gameTime);
    }
}
