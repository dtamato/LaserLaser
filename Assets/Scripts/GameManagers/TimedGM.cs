using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimedGM : BaseGM
{
    private Image timeBar;
	private float initialTime;
    private float initJoinDelay;
    public float gameTimer;


    new void Awake()
    {
        base.Awake();
        gameType = "TIMED";
		initialTime = gameTimer;
        initJoinDelay = joinGameDelay;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            //
            case (GAMESTATE.SETUP):
                if (SceneManager.GetActiveScene().buildIndex == mainGameSceneIndex)
                {
                    initializeGame();
                    timeBar = GameObject.Find("White Border").GetComponent<Image>();
                    timeBar.GetComponent<Image>().fillAmount = gameTimer / initialTime;
                    SetState(GAMESTATE.PREGAME);
                }

                break;

            //Pre-game runs from the time the game is initialized, until the end of the "Get Ready" countdown.
            case (GAMESTATE.PREGAME):
                if (playerCount < 1)
                {
                    joinGameDelay = initJoinDelay;
                }
                else
                {
                    joinGameDelay -= Time.deltaTime;
                    whiteBorder.GetComponent<Image>().fillAmount = joinGameDelay / initJoinDelay;

					if(whiteBorder.GetComponent<Image>().fillAmount < 0.25f) {

						whiteBorder.GetComponent<Image>().color = Color.red;
					}
					else if(whiteBorder.GetComponent<Image>().fillAmount < 0.5f) {

						whiteBorder.GetComponent<Image>().color = new Color(229, 83, 0);
                    }
					else
					{
                        whiteBorder.GetComponent<Image>().color = Color.yellow;
                    }
                }


                if ((joinGameDelay <= 0 || readyPlayers == playerCount) && playerCount > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        joinText[i].gameObject.SetActive(false);
                        inputText[i].gameObject.SetActive(false);
                    }
                    timeBar.color = Color.white;
                    
					readyText.SetActive(true);
					readyText.GetComponent<Text>().text = (Random.value < 0.5) ? "Fire Away, Player" : "Aim True, Player";

                    SetState(GAMESTATE.COUNTDOWN);
                    //Debug.Log(state);
                    StartCoroutine(CountDown()); //pre-game countdown. only activates after 'lobby countdown' has been executed.
                }

                break;

            //In-game runs from the time the "Get Ready" countdown ends, and the win condition is met.
            case (GAMESTATE.INGAME):
                if (GetPaused() == false)
                {
                    gameTimer -= Time.deltaTime;
                    timeBar.fillAmount = gameTimer / initialTime;
                }
                
                if (gameTimer < (0.25f * initialTime) && gameTimer > (0.22f * initialTime))
                {
                    //GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>().pitch = 1.05f;
					// Up spawn rate?	
                }

				if(gameTimer <= 10) {

					endCountdownText.gameObject.SetActive(true);
					endCountdownText.text = Mathf.FloorToInt(gameTimer + 1).ToString("00");
				}

				if (gameTimer <= 0) {
					
					endCountdownText.gameObject.SetActive(false);
                    GameOver();
				}
                break;

            //Post-game runs from the time the win condition is met, and the required players press return to menu.
            case (GAMESTATE.POSTGAME):

                break;
        }
    }
}