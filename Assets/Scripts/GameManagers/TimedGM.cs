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
                    timeBar = GameObject.Find("Time Bar").GetComponent<Image>();
                    timeBar.fillAmount = gameTimer / initialTime;
                    SetState(GAMESTATE.PREGAME);
                }

                break;

            //Pregame runs from the time the game is initialized, until the end of the "Get Ready" countdown.
            case (GAMESTATE.PREGAME):
                if (playerCount < 1)
                {
                    joinGameDelay = initJoinDelay;
                    joinCountdownText.text = "Not enough players!";
                }
                else
                {
                    joinGameDelay -= Time.deltaTime;
                    joinCountdownText.text = joinGameDelay.ToString("F1");
                }


                if ((joinGameDelay <= 0 || readyPlayers == playerCount) && playerCount > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        joinText[i].gameObject.SetActive(false);
                        inputText[i].gameObject.SetActive(false);
                    }
                    joinCountdownText.gameObject.SetActive(false);
                    readyText.SetActive(true);

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

                    GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioSource>().pitch = 1.1f;
                    timeBar.color = Color.red;
                }

                if (gameTimer <= 0)
                    GameOver();

                break;

            //Postgame runs from the time the win condition is met, and the required players press return to menu.
            case (GAMESTATE.POSTGAME):

                break;
        }
    }

    
}