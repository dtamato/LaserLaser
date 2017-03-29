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
                    timeBar.fillAmount = gameTimer / initialTime;
                    SetState(GAMESTATE.PREGAME);
                }

                break;

            //Pregame runs from the time the game is initialized, until the end of the "Get Ready" countdown.
            case (GAMESTATE.PREGAME):
                if (playerCount < 1)
                {
                    joinGameDelay = initJoinDelay;
                }
                else
                {
                    joinGameDelay -= Time.deltaTime;
                    joinCountdownImage.fillAmount = joinGameDelay / initJoinDelay;

					if(joinCountdownImage.fillAmount < 0.25f) {

						joinCountdownImage.color = Color.red;
					}
					else if(joinCountdownImage.fillAmount < 0.5f) {

						joinCountdownImage.color = Color.yellow;
					}
                }

				if(playerCount == 4 && joinUI.activeSelf) {

					joinUI.SetActive(false);
				}

                if ((joinGameDelay <= 0 || readyPlayers == playerCount) && playerCount > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        joinText[i].gameObject.SetActive(false);
                        inputText[i].gameObject.SetActive(false);
                    }
                    joinCountdownImage.gameObject.SetActive(false);
					joinUI.SetActive(false);
                    
					readyText.SetActive(true);
					readyText.GetComponent<Text>().text = (Random.value < 0.5) ? "Fire Away, Player" : "Aim True, Player";

                    SetState(GAMESTATE.COUNTDOWN);
                    //Debug.Log(state);
                    StartCoroutine(CountDown()); //pregame countdown. only activates after 'lobby countdown' has been executed.
                }

                break;

            //Ingame runs from the time the "Get Ready" countdown ends, and the win condition is met.
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

            //Postgame runs from the time the win condition is met, and the required players press return to menu.
            case (GAMESTATE.POSTGAME):

                break;
        }
    }
}