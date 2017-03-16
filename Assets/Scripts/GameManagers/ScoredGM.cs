using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoredGM : BaseGM
{
    /*
    #region Variables

    //References for initialization process.
    private bool initialized = false;
    private bool enteredLobby = false;
    private Text timeText;

    //Score to win the game.
    public int objectiveScore;

    #endregion

    new void Awake ()
    {
        base.Awake();
    }

        
    void Update()
    {
        #region Lobby Scene
        switch (state)
        {
            case (GAMESTATE.PREGAME):

                //When the GM enters the game scene, initialize the game.

                //Run the base game initialization, all GMs run this.
                base.initializeGame();

                //Remove the timer.
                GameObject.Find("Time Bar").SetActive(false);
                //timeText = GameObject.Find("TimeBar").GetComponent<Text>();
                //timeText.text = "Target: " + objectiveScore;

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
                state = GAMESTATE.INGAME;
                break;

            case (GAMESTATE.INGAME):

                //Core game loop once in the game scene. inGame is set in BaseGM.initializeGame().
                //If in TeamMode update the team's scores.
                if (gameMode == "TB")
                {
                    HUDText[0].text = "Team 1: " + team1Score;
                    HUDText[1].text = "Team 2: " + team2Score;
                }

                //Check for a winner in FFA mode.
                if (gameMode == "FFA")
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int score = playerList[i].getScore();
                        if (score >= objectiveScore)
                        {
                            Transform ballTransform = playerList[i].obj.GetComponent<Cannon>().GetLaser().transform;
                            Camera.main.GetComponent<CameraEffects>().SetZoomTarget(ballTransform);
                            GameOver();
                        }
                    }
                }
                //Check for a winner in TB mode.
                else
                {
                    if (team1Score >= objectiveScore || team2Score >= objectiveScore)
                        GameOver();
                }

                break;
            case (GAMESTATE.POSTGAME):
                break;
        }

        //When first entering the lobby from the menu scene.
        /*if (enteredLobby == false && SceneManager.GetActiveScene().buildIndex == LobbySceneIndex)
        {
            lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
            lobbyManager.gameType = gameMode;
            lobbyManager.SwitchTeamMode();
            enteredLobby = true;
        }

        #endregion
    
    }
    */
}
