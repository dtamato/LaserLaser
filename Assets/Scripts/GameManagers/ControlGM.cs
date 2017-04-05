using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class ControlGM : BaseGM {

	#region Variables

	//References for initialization process.
	[SerializeField] float gameLength = 60f;
	private bool initialized = false;
	private bool enteredLobby = false;
	private float gameTimer;
	private Image gameTimerBar;

	//Score to win the game.
	public int objectiveScore;

	#endregion

	new void Awake ()
	{
		base.Awake();
	}

	void Update()
	{
		#region Main Game Scene

	    switch (state)
	    {
            case (GAMESTATE.SETUP):
                base.controlInitializeGame();

                gameTimerBar = GameObject.Find("Time Bar").GetComponent<Image>();
                GameObject.Find("GetReadyText").gameObject.SetActive(false);
                GameObject.Find("Item Spawner").gameObject.SetActive(false);
                GameObject.Find("Powerup Spawner").gameObject.SetActive(false);

                gameTimer = gameLength;
                SetState(GAMESTATE.INGAME);
                break;
            case (GAMESTATE.INGAME):

                gameTimer -= Time.deltaTime;
                gameTimerBar.fillAmount = gameTimer / gameLength;

                if (gameTimer <= 0)
                {

                    GameOver();

                    // Calculate who won
                    GameObject[] controlBouncers = GameObject.FindGameObjectsWithTag("ControlBouncer");

                    int redBouncers = 0;
                    int blueBouncers = 0;

                    for (int i = 0; i < controlBouncers.Length; i++)
                    {

                        float redAmount = controlBouncers[i].GetComponent<SpriteRenderer>().color.r;
                        float blueAmount = controlBouncers[i].GetComponent<SpriteRenderer>().color.b;

                        if (redAmount > blueAmount)
                        {

                            redBouncers++;
                        }
                        else if (blueAmount > redAmount)
                        {

                            blueBouncers++;
                        }
                    }

                    bool redWon = (redBouncers > blueBouncers) ? true : false;
                    Color winningColor = redWon ? Color.red : Color.blue;
                    string winningTeam = redWon ? "Red Team" : "Blue Team";

                    // Display results
                    /*gameOverPanel.transform.Find("Border").GetComponent<Image>().color = winningColor;
                    gameOverPanel.transform.Find("WinnerText").GetComponent<Text>().color = winningColor;
                    gameOverPanel.transform.Find("WinnerText").GetComponent<Text>().text = winningTeam + " Wins!";
                    gameOverPanel.SetActive(true);*/
                }

                break;

	    }

		#endregion
	}
}
