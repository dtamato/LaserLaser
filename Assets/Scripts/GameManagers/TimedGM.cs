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

    // Update is called once per frame
    void Update ()
    {
        //When first entering the lobby from the menu scene.
        if (enteredLobby == false) {
            lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
            lobbyManager.gameType = gameMode;
            lobbyManager.SwitchTeamMode();
            enteredLobby = true;
        }

        #region MainGame Scene

        //When the GM enters the game scene, initialize the game.
        if (!initialized && SceneManager.GetActiveScene().buildIndex == mainGameSceneIndex) {
            //Run the base game intialization, all GMs run this.
            base.initializeGame();

            //Put the time on the clock.
            timeText = GameObject.Find("TimeText").GetComponent<Text>();
            timeText.text = gameTimer.ToString("F1");

            //Ensures this process runs once.
            initialized = true;
        }


        //Core game loop once in the game scene. inGame is set in BaseGM.initializeGame().
        if (inGame) {

        }

        #endregion
    }
}
