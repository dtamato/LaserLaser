using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TimedGM : BaseGM
{
    #region Variables
    //References for initialization process.
    private bool initialized = false;
    private bool enteredLobby = false;
    private Text timeText;
    //How long the game will run for.
    public float gameTimer;
    #endregion

    void Awake()
    {
        base.Awake();
    }


    // Update is called once per frame
    void Update()
    {
        //When first entering the lobby from the menu scene.
        if (enteredLobby == false && SceneManager.GetActiveScene().buildIndex == LobbySceneIndex)
        {
            lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
            lobbyManager.gameType = gameMode;
            lobbyManager.SwitchTeamMode();
            enteredLobby = true;
        }
        #region MainGame Scene
        //When the GM enters the game scene, initialize the game.
        else if (!initialized && SceneManager.GetActiveScene().buildIndex == mainGameSceneIndex)
        {
            //Run the base game intialization, all GMs run this.
            base.initializeGame();
            
            //Put the time on the clock.
            timeText = GameObject.Find("TimeText").GetComponent<Text>();
            timeText.text = gameTimer.ToString("F1");

            //Deactivate inactive player's scores. FFA Only.
            if (gameMode == "FFA")
            {
                for (int i = 0; i <= 3; i++)
                {
                    GameObject scorebar = GameObject.Find("PlayerScore" + i);
                    if (!playerList[i].active())
                        scorebar.SetActive(false);
                }
            }

            //Ensures this process runs once.
            initialized = true;
        }
        //Core game loop once in the game scene. inGame is set in BaseGM.initializeGame().
        else if (inGame && startGame && !gameOver)
        {
            gameTimer -= Time.deltaTime;
            timeText.text = gameTimer.ToString("F1");

            //If in TeamMode update the team's scores.
            if (gameMode == "TB")
            {
                HUDText[0].text = "Team 1: " + team1Score;
                HUDText[1].text = "Team 2: " + team2Score;
            }

            if (gameTimer <= 0)
                GameOver();
        }

        #endregion
    }
}