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
    private Text timeText;

    //How long the game will run for.
    public float gameTimer;

    #endregion

    // Update is called once per frame
    void Update ()
    {
        //All lobby activity is handled in baseGM.cs.

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
