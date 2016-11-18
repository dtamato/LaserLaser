using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public List <bool> playerState;
    public List <int> playerScores;

    //Boolean flags.
    public bool inGame = false;        //True when in Main Game (Build Code 1).
    public bool startGame = false;     //True when the 5 second countdown has passed.

    //Timer variables.
    private Text timeText;
    public float timer;
    private float countDown;
    private float delay;
        
    private GameObject gameOverPanel;

    //Ensures there is only one instance of the gameManager, and it isn't destroyed when changing scenes.
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        //Initialize the player state list and player score list.
        playerState = new List<bool>(4);
        playerScores = new List<int>(4);
        for (int i = 0; i < 4; i++) {
            playerState.Add(false);
            playerScores.Add(0);
        }
    }

    void Start () {
	
	}
	
	void Update ()
    {
        //Resets game state when returning to lobby after a game.
        if (SceneManager.GetActiveScene().buildIndex == 1 && inGame) {
            inGame = false;

            playerState = new List<bool>(4); //re-initializes playerlist
            playerScores = new List<int>(4);
            for (int i = 0; i < 4; i++)
            {
                playerState.Add(false);
                playerScores.Add(0);
            }
        }

        //Runs once when entering the Main Game Scene.
        if (SceneManager.GetActiveScene().buildIndex == 2 && !inGame) {
            //Initialize the timer.
            inGame = true;                                                  //Sets the game state to in game.
            timeText = GameObject.Find("TimeText").GetComponent<Text>();
            timer = 90.0f;                                                  //Game timer, set to desired game length.
            delay = 3.0f;                                                   //Countdown / grace period. Input activates after this many seconds.
            timeText.text = timer.ToString("F1");                           //Put the time on the clock.
            
            //Initialize the game over panel.
            gameOverPanel = GameObject.Find("GameOverPanel");
            for (int i = 0; i < 4; i++) {
                GameObject scoreBar = GameObject.Find("ScoreBar" + i);
                GameObject score = GameObject.Find("FinalScore" + i);
                //Check if each player is active, and deactivate the score summaries for those who aren't active
                if (!playerState[i]) { 
                    scoreBar.SetActive(false);
                    score.SetActive(false);
                }
            }
            gameOverPanel.SetActive(false);
            
            //Start the countdown to gameplay.
            StartCoroutine("CountDown");
        }
        
        //Runs every frame once the game has been initialized.
        if (inGame && startGame) {
            timer -= Time.deltaTime;
            timeText.text = timer.ToString("F1");
        }

		if (startGame && timer <= 30f) {

			GameObject.Find ("Audio Kitty").GetComponent<AudioSource> ().pitch = 1.25f;
		}

        //When the timer runs out, the game is over.
        if (startGame && timer <= 0.0f) {
            startGame = false;
            timeText.text = "0.0";
            GameOver();
        }
        
	}

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(delay);
		GameObject.Find("Get Ready Text").SetActive (false);
        startGame = true;       //Once this is toggled, players have input.
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);

		GameObject topBarGroup = GameObject.Find ("Top Bar Group");
		if (topBarGroup) {
			topBarGroup.SetActive (false);
		}

        //Iterate through each active player.
        for (int i = 0; i < 4; i++) {
            if (playerState[i]) {
                GameObject player = GameObject.Find("Cannon" + i);
                player.GetComponent<Cannon>().gameOver = true;
                playerScores[i] = GameObject.Find("Laser" + i).GetComponent<Laser>().score;                                                     //Save the player's score.
                GameObject score = GameObject.Find("FinalScore" + i);                                                                           
                GameObject scoreBar = GameObject.Find("Bar" + i);
                score.GetComponent<Text>().text = (playerScores[i]).ToString();                                                                 //Set the score text.
                scoreBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, (3 * playerScores[i]));      //Resize the score bar.
            }
            else
                playerScores[i] = 0;    //If no player was active in a slot, set their score to 0.
        }

        //Determine who was the winner.
        int highScore = 0;
        int winner = 0;
        for (int i = 0; i < 4; i++) {
            if (playerScores[i] > highScore) {
                highScore = playerScores[i];
                winner = i;
            }
        }

        //Set the Winner Text.
        Text winnerText = GameObject.Find("WinnerText").GetComponent<Text>(); //This should be changed, its unsafe. If winnertext is not in scene, game breaks.
        winnerText.text = ("Player " + (winner + 1) + " Wins!");

        //Set the panel color to that of the winner.
        Image panel1 = GameObject.Find("GameOverPanel").GetComponent<Image>();
		panel1.color = new Color32 (74, 68, 249, 255);
    }

    //Called from Cannon.cs.
    public void ReturnToLobby()
    {
        //Reset the player states when returning to lobby.
        for (int i = 0; i < 4; i++)
            playerState[i] = false;

        SceneManager.LoadScene(0);
    }

    //Called from Cannon.cs. Sets whether the player is active or not.
    //Called in Lobby.
    public void setPlayerState(int playerID)
    {
        playerState[playerID] = !playerState[playerID];
    }

    //Called from Cannon.cs. Returns whether the player is active or not.
    //Called in Main Game.
    public bool getPlayerState(int playerID)
    {
        return playerState[playerID];
    }
}
