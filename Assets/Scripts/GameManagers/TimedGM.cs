using UnityEngine;
using UnityEngine.UI;

public class TimedGM : BaseGM
{
    private Image timeBar;
	private float initialTime;
    public float gameTimer;

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
            case (GAMESTATE.PREGAME):

                //Put the time on the clock.
                timeBar = GameObject.Find("Time Bar").GetComponent<Image>();
                timeBar.fillAmount = gameTimer / initialTime;

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
                break;

            //
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

             //
            case (GAMESTATE.POSTGAME):

                //No functionality yet.

                break;
            
            //
            default:
                base.initializeGame();
                break;
        }
    }
}