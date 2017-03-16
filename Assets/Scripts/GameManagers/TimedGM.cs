using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimedGM : BaseGM
{
    private Image timeBar;
	private float initialTime;
    public float gameTimer;
    float timeCount = 0;

    new void Awake()
    {
        base.Awake();
		initialTime = gameTimer;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            //Pregame runs from the time the game is initialized, until the end of the "Get Ready" countdown.
            case (GAMESTATE.PREGAME):

                //No functionality yet.
                
                Debug.Log(timeCount);
                if (timeCount >= joinGameDelay || GetReadyPlayers() == playerCount) //I did see the enum, but it didn't work so I had to use this.
                {
                    state = GAMESTATE.INGAME;
                }
                else
                {
                    timeCount += Time.deltaTime;
                }

                break;

            //Ingame runs from the time the "Get Ready" countdown ends, and the win condition is met.
            case (GAMESTATE.INGAME):

                gameTimer -= Time.deltaTime;
                timeBar.fillAmount = gameTimer / initialTime;

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

                //No functionality yet.

                break;
            case (GAMESTATE.SETUP): //default case wasn't working?
                if (SceneManager.GetActiveScene().buildIndex == mainGameSceneIndex)
                {
                    SetState(GAMESTATE.PREGAME);
                    Debug.Log(state);
                    initializeGame();
                    timeBar = GameObject.Find("Time Bar").GetComponent<Image>();
                    timeBar.fillAmount = gameTimer / initialTime;
                }
                    
                

                break;

            //Initial setup of scene, before pregame.
            default:
                
                break;
        }
    }
}