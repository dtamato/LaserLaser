using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;


public class BaseGM : MonoBehaviour
{
    #region Variables

    //Scene build index integers.
    public int menuSceneIndex = 0;
    public int LobbySceneIndex = 1;
    public int mainGameSceneIndex = 2;
   
    //External References.
    public static BaseGM instance = null;
    protected LobbyManager lobbyManager;
    protected List<Text> HUDText;
    [SerializeField] protected List<PlayerDef> playerList;
	protected GameObject[] activePlayersArray;
    protected GameObject gameOverPanel;
    protected GameObject introText;
	protected GameObject whiteBorder;
    protected List<GameObject> spawns;
    public GameObject playerObj;

    //Boolean flags and other metrics.
    public bool inGame = false;        //True when in Main Game.
    public bool startGame = false;     //True once the startGameDelay / grace period has elapsed.
    public bool gameOver = false;
    public int playerCount = 0;
    public string gameMode;
    public int team1Score, team2Score;  //Only used in TB mode.

    //Timer variables.
    public float startGameDelay;
    
    #endregion

    //Class defining a player's attributes. Used to carry player preferences into the game scene from lobby.
    [System.Serializable]
    public class PlayerDef
    {
        public GameObject obj;
        [SerializeField]
        private int ID;
        [SerializeField]
        private bool isActive;
        [SerializeField]
        private int sensitivity;
        [SerializeField]
        private bool inverted;
        [SerializeField]
        private int team;
        [SerializeField]
        private Color color;
        [SerializeField]
        private int score;
        [SerializeField]
        private Color teamColor;

        //Constructor sets defaults for player settings.
        public PlayerDef()
        {
            ID = -1;
            isActive = false;
            sensitivity = 1;
            inverted = false;
            team = -1;
            color = Color.grey;
            teamColor = new Color(0.8f, 0.8f, 0.8f, 0f);
            score = 0;
        }
        //List of setters for private members.
#region Setters
        public void setID(int value)
        {
            ID = value;
        }
        public void setActive(bool state)
        {
            isActive = state;
        }
        public void setSensitivity(int value)
        {
            sensitivity = value;
        }
        public void setInverted(bool state)
        {
            inverted = state;
        }
        public void setTeam(int value)
        {
            team = value;
        }
        public void setColor(Color value)
        {
            color = value;
        }
        public void setScore(int value)
        {
            score = value;
        }

        public void setTeamColor(Color value)
        {
            teamColor = value;
        }
        #endregion

#region Getters
        //List of getters for private variables.
        public int getID()
        {
            return ID;
        }
        public bool active()
        {
            return isActive;
        }
        public int getScore()
        {
            return score;
        }
        public int getSensitivity()
        {
            return sensitivity;
        }
        public bool getInverted()
        {
            return inverted;
        }
        public int getTeam()
        {
            return team;
        }
        public Color getColor()
        {
            return color;
        }
        public Color getTeamColor()
        {
            return teamColor;
        }
        #endregion

        //Reset function reverts preferences to default if player leaves lobby.
        public void reset()
        {
            ID = -1;
            isActive = false;
            sensitivity = 5;
            inverted = false;
            team = -1;
            color = Color.grey;
            teamColor = new Color(0.8f,0.8f,0.8f,0f);
        }
    }

    //Called immediately when game manager is instantiated in Menu.
    protected void Awake()
    {
        //Ensures there is only one instance of the gameManager, and it isn't destroyed when changing scenes.
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        //Initialize HUD Text List and Spawn Points, will be referenced upon entering game scene.
        HUDText = new List<Text>(4);
        spawns = new List<GameObject>(4);
        //Initialize Player List, holds preferences to instantiate each player in game scene.
        playerList = new List<PlayerDef>(4);
        for (int i = 0; i <= 3; i++)
        {
            playerList.Add(new PlayerDef());
            HUDText.Add(null);
            spawns.Add(null);
        }
        ///
        /// Set gameMode based on main menu preferences. Must be set to opposite of intended mode. Running the switch function in LobbyManager sets it properly.
        ///
    }

    //These functions are called based on player input in the lobby.
    #region Lobby Scene

    //Called from Cannon.cs when a player enters the lobby.
    public void playerJoin(int pID, int teamNo, Color color, Color teamColor)
    {
        playerList[pID].setID(pID);
        playerList[pID].setTeam(teamNo);
        playerList[pID].setColor(color);
        playerList[pID].setTeamColor(teamColor);
        playerList[pID].setActive(true);
    }
    //Called from Cannon.cs when a player leaves the lobby.
    public void playerLeave(int pID)
    {
        playerList[pID].reset();
    }
    //Called from Cannon.cs when a player changes their color.
    public void setColor(int pID, Color color)
    {
        playerList[pID].setColor(color);
    }

    //Called from Cannon.cs when a player changes their sensitivity.
    public void setSensitivity(int pID, int sens)
    {
        playerList[pID].setSensitivity(sens);
    }
    //Called from Cannon.cs when a player changes their invertedness.
    public void setInvert(int pID, bool state)
    {
        playerList[pID].setInverted(state);
    }
    //Called from Cannon.cs when a player changes their team in Team Play.
    public void setTeam(int pID, int team)
    {
        playerList[pID].setTeam(team);
    }
    public void setID(int pID)
    {
        playerList[pID].setID(pID);
    }

    public void setTeamColour(int pId, Color color)
    {
        playerList[pId].setTeamColor(color);
    }
    #endregion
    #region MainGame Scene
    //Called when entering game scene, initializes players, HUD and timer.
    protected void initializeGame()
    {
        //Set game state to In Game.
        inGame = true;

        //Reference members of HUD and Spawns.
        for (int i = 0; i <= 3; i++)
        {
            HUDText[i] = GameObject.Find("PlayerScore" + i).GetComponent<Text>();
            spawns[i] = GameObject.Find("SP" + i);
        }

        //Reference the game over panel and get ready text.
        introText = GameObject.Find("GetReadyText");
		whiteBorder = GameObject.Find ("White Border");
        gameOverPanel = GameObject.Find("GameOverPanel");
        gameOverPanel.SetActive(false);

        //Set the locations of the spawn points, relative to the parent object located at the lower left corner of the arena.
        float dist = 19.4f / (2 * playerCount);
        for (int i = 0; i < playerCount; i++)
        {
            float newX = (dist * i) + dist;
            spawns[i].transform.position.Set(newX, 0, 0);
        }

        //Instantiate the player objects, and assign their preferences.
        for (int i = 0; i < playerCount; i++)
        {
            //Spawn the player object.
            playerList[i].obj = Instantiate(playerObj, spawns[i].transform.position, spawns[i].transform.rotation) as GameObject;

            //Pair the CannonCustomization script.
            CannonCustomization prefs = playerList[i].obj.GetComponent<CannonCustomization>();

            //Apply the settings to the player object from the GM stored preferences.
            prefs.myID = playerList[i].getID();
            prefs.sensitivity = playerList[i].getSensitivity();
            prefs.inverted = playerList[i].getInverted();
            prefs.myColor = playerList[i].getColor();
            prefs.team = playerList[i].getTeam();
            prefs.myTeamColor = playerList[i].getTeamColor();

            //Set the player's ID, and pair their controls.
            playerList[i].obj.GetComponent<Cannon>().playerId = prefs.myID;
            playerList[i].obj.GetComponent<Cannon>().rewiredPlayer = ReInput.players.GetPlayer(prefs.myID);

            //Apply all the colour changes to the player's laser and cannon.
            Laser laserScript = playerList[i].obj.transform.Find("Laser").GetComponent<Laser>();
            laserScript.myPlayerID = prefs.myID;
            laserScript.myTeam = prefs.team;
            playerList[i].obj.transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = prefs.myColor; //Laser color
            playerList[i].obj.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = prefs.myColor; //Trail renderer color
            playerList[i].obj.GetComponentInChildren<SpriteRenderer>().color = prefs.myColor; //Cannon color
            
            //If the mode is Team, set the player's colourband, otherwise disable it.
            if (gameMode == "TB") {
                playerList[i].obj.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = prefs.myTeamColor;
                laserScript.scoreText = GameObject.Find("PlayerScore" + (prefs.team - 1)).GetComponent<Text>();   //Laser reference for team score updates.
            }
            //Else, in FFA set each player's score and combo color seperately.
            else {
                playerList[i].obj.transform.Find("ColourBand").gameObject.SetActive(false);
                GameObject.Find("PlayerScore" + i).GetComponent<Text>().color = prefs.myColor; //Score color
                laserScript.scoreText = GameObject.Find("PlayerScore" + prefs.myID).GetComponent<Text>();   //Laser reference for score updates.
            }

            Debug.Log("player added");
        }

        //In team mode, only use 2 score overlays, one per team.
        if (gameMode == "TB")
        {
            //Disable unused HUD.
            GameObject.Find("PlayerScore" + 2).SetActive(false);
            GameObject.Find("PlayerScore" + 3).SetActive(false);

            GameObject.Find("PlayerScore" + 0).GetComponent<Text>().text = "Team 1: " + team1Score;
            GameObject.Find("PlayerScore" + 0).GetComponent<Text>().color = Color.blue;
            GameObject.Find("PlayerScore" + 1).GetComponent<Text>().text = "Team 2: " + team2Score;
            GameObject.Find("PlayerScore" + 1).GetComponent<Text>().color = Color.red;
        }

        FillActivePlayersArray ();

        //Start the countdown to gameplay.
        StartCoroutine("CountDown");
    }

	void FillActivePlayersArray () {

		List<GameObject> activePlayersList = new List<GameObject> ();

		for (int i = 0; i < playerList.Count; i++) {

			if (playerList [i].obj != null) {

				activePlayersList.Add (playerList [i].obj);
			}
		}

		activePlayersArray = activePlayersList.ToArray ();
	}

    //Brief grace period before diamonds start spawning.
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(startGameDelay);
        introText.SetActive(false);
        startGame = true;       //Once this is toggled, players have input.
    }

    //Called from each subclass GM.
    public void GameOver()
    {
        gameOver = true;
        
        //Determine who was the winner.
        int highScore = 0;
        int winner = 0;

        //Display the results.
        gameOverPanel.SetActive(true);

        //Scoring for FFA.
        if (gameMode == "FFA")
        {
            for (int i = 0; i < 4; i++)
            {
                int score = playerList[i].getScore();
                if (score > highScore)
                {
                    highScore = score;
                    winner = i;
                }
            }

            //Set the Winner Text.
            GameObject.Find("WinnerText").GetComponent<Text>().text = ("Player " + (winner + 1) + " Wins!");
            GameObject.Find("FinalScoreText").GetComponent<Text>().text += highScore;

            //Set the panel color to that of the winner.
            GameObject.Find("GameOverPanel").GetComponent<Image>().color = playerList[winner].getColor();
        }

        //Scoring for TB.
        else
        {
            //Determine winning team.
            winner = team1Score > team2Score ? 1 : 2;
            highScore = team1Score > team2Score ? team1Score : team2Score;

            //Set the Winner Text.
            GameObject.Find("WinnerText").GetComponent<Text>().text = ("Team " + (winner) + " Wins!");
            GameObject.Find("FinalScoreText").GetComponent<Text>().text += highScore;

            //Set the panel color to that of the winner.
            Color winColor = winner == 1 ? Color.blue : Color.red;
            GameObject.Find("GameOverPanel").GetComponent<Image>().color = winColor;
        }

        

        Debug.Log("Game Over, Results Displayed.");
    }
    #endregion
    //Called from Cannon.cs.
    public void changeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    //Returns to menu and destroys GM and rewired manager.
    public void returnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
        Destroy(GameObject.Find("Rewired Input Manager"));
        Debug.Log("Goodbye cruel world.");
        Destroy(gameObject);
    }

    //Called from Laser.cs
    public void addScore(int pID, int score)
    {
        playerList[pID].setScore(score);
		UpdateWhiteBorderFFA ();
        HUDText[pID].text = "P1- " + score.ToString("00");
    }

	void UpdateWhiteBorderFFA () {

		int winningScore = -1;
		int winningPlayerIndex = -1;

		for (int i = 0; i < playerList.Count; i++) {

			if (playerList [i].getScore () > winningScore) {

				winningScore = playerList [i].getScore ();
				winningPlayerIndex = i;
				whiteBorder.GetComponent<SpriteRenderer> ().color = playerList [i].getColor ();
			}
			else if (playerList [i].getScore () == winningScore) {

				whiteBorder.GetComponent<SpriteRenderer> ().color = Color.white;
			}
		}
	}

	public void addToTeamScore(bool isTeam1) {

		if (isTeam1) {

			team1Score++;
		}
		else {

			team2Score++;
		}

		UpdateWhiteBorderTB ();
	}

	void UpdateWhiteBorderTB () {

		if (team1Score > team2Score) {

			whiteBorder.GetComponent<SpriteRenderer> ().color = Color.blue;
		}
		else if (team2Score > team1Score) {

			whiteBorder.GetComponent<SpriteRenderer> ().color = Color.red;
		}
		else {

			whiteBorder.GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}

	public GameObject[] GetActivePlayers () {

		return activePlayersArray;
	}
}