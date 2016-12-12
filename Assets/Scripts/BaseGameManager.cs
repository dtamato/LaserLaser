using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseGameManager : MonoBehaviour
{
    //External References.
    public static BaseGameManager instance = null;
    protected List<Text> HUDText;
    [SerializeField] protected List<PlayerDef> playerList;
    protected GameObject gameOverPanel;
    protected GameObject introText;

    //Boolean flags.
    public bool inGame = false;        //True when in Main Game.
    public bool startGame = false;     //True once the startGameDelay / grace period has elapsed.

    //Timer variables.
    public float gameTimer;
    public float startGameDelay;
        
    //Class defining a player's attributes. Used to carry player preferences into the game scene from lobby.
    public class PlayerDef
    {
        private int ID;
        private bool isActive;
        private int sensitivity;
        private bool inverted;
        private int team;
        private Color color;
        private int score;

        //Constructor sets defaults for player settings.
        public PlayerDef() {
            ID = -1;
            isActive = false;
            sensitivity = 5;
            inverted = false;
            team = -1;
            color = Color.grey;
            score = 0;
        }

        //List of setters for private members.
        public void setID (int value) {
            ID = value;
        }

        public void setActive (bool state) {
            isActive = state;
        }

        public void setSensitivity (int value) {
            sensitivity = value;
        }

        public void setInverted (bool state) {
            inverted = state;
        }

        public void setTeam (int value) {
            team = value;
        }

        public void setColor (Color value) {
            color = value;
        }

        public void setScore (int value) {
            score = value;
        }

        //List of getters for private variables.
        public bool active() {
            return isActive;
        }

        public int getScore() {
            return score;
        }

        //Reset function reverts preferences to default if player leaves lobby.
        public void reset()
        {
            ID = -1;
            isActive = false;
            sensitivity = 5;
            inverted = false;
            team = -1;
            color = Color.grey;
        }
    }


    void Awake()
    {
        //Ensures there is only one instance of the gameManager, and it isn't destroyed when changing scenes.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        //Initialize HUD Text List, will be referenced upon entering game scene.
        //Index 0-3 for player scores, Index 4 for timer.
        HUDText = new List<Text>(5);

        //Initialize Player List, holds preferences to instantiate each player in game scene.
        playerList = new List<PlayerDef>(4);
        for (int i = 0; i <= 3; i++) {
            playerList[i] = new PlayerDef();
        }    
    }

	
    //Called when entering game scene, initializes players, HUD and timer.
    protected void initializeGame()
    {
        //Set game state to In Game.
        inGame = true;

        //Reference members of HUD.
        for (int i = 0; i <= 3; i++) {
            HUDText[i] = GameObject.Find("PlayerScore" + i).GetComponent<Text>();
        }
        HUDText[4] = GameObject.Find("TimeText").GetComponent<Text>();
        introText = GameObject.Find("GetReadyText");

        //Put the time on the clock.
        HUDText[4].text = gameTimer.ToString("F1");

        //Initialize the game over panel. Likely to be altered / removed in future commit.
        gameOverPanel = GameObject.Find("GameOverPanel");
        for (int i = 0; i <= 3; i++) {
            GameObject scoreBar = GameObject.Find("ScoreBar" + i);
            GameObject score = GameObject.Find("FinalScore" + i);
            //Check if each player is active, and deactivate the score summaries for those who aren't active
            if (!playerList[i].active())
            {
                scoreBar.SetActive(false);
                score.SetActive(false);
            }
        }
        gameOverPanel.SetActive(false);

        /////////////////


        //Assign Player Preferences.


        ////////////////

        //Start the countdown to gameplay.
        StartCoroutine("CountDown");
    }
        
    //Brief grace period before diamonds start spawning.
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(startGameDelay);
		introText.SetActive (false);
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
        for (int i = 0; i <= 3; i++) {
            //If the player is active.
            if (!playerList[i].active()) {
                
                ///////////////////

                //Current gameOver handling, likely to be altered or removed in future commit.
                int score = GameObject.Find("Laser" + i).GetComponent<Laser>().score;
                playerList[i].setScore(score);
                GameObject scoreObj = GameObject.Find("FinalScore" + i);
                GameObject scoreBar = GameObject.Find("Bar" + i);
                scoreObj.GetComponent<Text>().text = (playerList[i].getScore()).ToString();
                scoreBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, (3 * playerList[i].getScore()));

                ///////////////////
            }
            else
                playerList[i].setScore(0);    //If no player was active in a slot, set their score to 0.
        }

        ///////////////////

        //Current gameOver handling, likely to be altered or removed in future commit.

        //Determine who was the winner.
        int highScore = 0;
        int winner = 0;
        for (int i = 0; i < 4; i++) {
            int score = playerList[i].getScore();
            if (score > highScore) {
                highScore = score;
                winner = i;
            }
        }

        //Set the Winner Text.
        Text winnerText = GameObject.Find("WinnerText").GetComponent<Text>();
        winnerText.text = ("Player " + (winner + 1) + " Wins!");

        //Set the panel color to that of the winner.
        Image panel1 = GameObject.Find("GameOverPanel").GetComponent<Image>();
		panel1.color = new Color32 (74, 68, 249, 255);

        ///////////////////
    }
}
