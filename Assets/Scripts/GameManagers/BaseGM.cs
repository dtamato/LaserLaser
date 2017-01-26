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
    protected GameObject gameOverPanel;
    protected GameObject introText;
    protected List<GameObject> spawns;
    public GameObject playerObj;
    
    //Boolean flags and other metrics.
    public bool inGame = false;        //True when in Main Game.
    public bool startGame = false;     //True once the startGameDelay / grace period has elapsed.
    public bool gameOver = false;
    public int playerCount = 0;
    public string gameMode;
    
    //Timer variables.
    public float startGameDelay;
    #endregion
    
    //Class defining a player's attributes. Used to carry player preferences into the game scene from lobby.
    [System.Serializable]
    public class PlayerDef
    {
        public GameObject obj;
        [SerializeField] private int ID;
        [SerializeField] private bool isActive;
        [SerializeField] private int sensitivity;
        [SerializeField] private bool inverted;
        [SerializeField] private int team;
        [SerializeField] private Color color;
        [SerializeField] private int score;
        
        //Constructor sets defaults for player settings.
        public PlayerDef()
        {
            ID = -1;
            isActive = false;
            sensitivity = 1;
            inverted = false;
            team = -1;
            color = Color.grey;
            score = 0;
        }
        //List of setters for private members.
        public void setID(int value) {
            ID = value;
        }
        public void setActive(bool state) {
            isActive = state;
        }
        public void setSensitivity(int value) {
            sensitivity = value;
        }
        public void setInverted(bool state) {
            inverted = state;
        }
        public void setTeam(int value) {
            team = value;
        }
        public void setColor(Color value) {
            color = value;
        }
        public void setScore(int value) {
            score = value;
        }
        
        //List of getters for private variables.
        public int getID() {
            return ID;
        }
        public bool active() {
            return isActive;
        }
        public int getScore() {
            return score;
        }
        public int getSensitivity() {
            return sensitivity;
        }
        public bool getInverted() {
            return inverted;
        }
        public int getTeam() {
            return team;
        }
        public Color getColor() {
            return color;
        }
        
        //Reset function reverts preferences to default if player leaves lobby.
        public void reset()
        {
            ID = -1;
            isActive = false;
            sensitivity = 1;
            inverted = false;
            team = -1;
            color = Color.grey;
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
    public void playerJoin(int pID, int teamNo, Color color)
    {
        playerList[pID].setID(pID);
        playerList[pID].setTeam(teamNo);
        playerList[pID].setColor(color);
        playerList[pID].setActive(true);
    }

    //A list of public functions, accessed from Cannon.cs to record player data.
    public void playerLeave(int pID)
    {
        playerList[pID].reset();
    }

    public void setColor(int pID, Color color) {
        playerList[pID].setColor(color);
    }

    public void setSensitivity(int pID, int sens) {
        playerList[pID].setSensitivity(sens);
    }

    public void setInvert(int pID, bool state) {
        playerList[pID].setInverted(state);
    }

    public void setTeam(int pID, int team) {
        playerList[pID].setTeam(team);
    }

    public void setID(int pID)
    {
        playerList[pID].setID(pID);
    }

    #endregion

    //These functions are called once the transition to the main game has occured.
    #region MainGame Scene
    
    //Called when entering game scene, initializes players, HUD and timer.
    protected void initializeGame()
    {
        Debug.Log(playerCount);
        //Make reference to In Game HUD.
        inGame = true;
        introText = GameObject.Find("GetReadyText");
        gameOverPanel = GameObject.Find("GameOverPanel");

        for (int i = 0; i <= 3; i++) {
            HUDText[i] = GameObject.Find("PlayerScore" + i).GetComponent<Text>();
            spawns[i] = GameObject.Find("SP" + i);
        }

        //Initialize the game over panel. Likely to be altered / removed in future commit.
        for (int i = 0; i <= 3; i++)
        {
            GameObject scoreBar = GameObject.Find("ScoreBar" + i);
            GameObject score = GameObject.Find("FinalScore" + i);

            //Check if each player is active, and deactivate the score summaries for those who aren't active
            if (!playerList[i].active()) {
                scoreBar.SetActive(false);
                score.SetActive(false);
            }
        }

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
            playerList[i].obj = Instantiate(playerObj, spawns[i].transform.position, spawns[i].transform.rotation) as GameObject;
            CannonCustomization prefs = playerList[i].obj.GetComponent<CannonCustomization>();
            prefs.myID = playerList[i].getID();
            prefs.sensitivity = playerList[i].getSensitivity();
            prefs.inverted = playerList[i].getInverted();
            prefs.myColor = playerList[i].getColor();
            prefs.team = playerList[i].getTeam();
            playerList[i].obj.GetComponent<Cannon>().playerId = prefs.myID;
            playerList[i].obj.GetComponent<Cannon>().rewiredPlayer = ReInput.players.GetPlayer(prefs.myID);

            Laser laserScript = playerList[i].obj.transform.Find("Laser").GetComponent<Laser>();
            laserScript.myPlayerID = prefs.myID;
            laserScript.scoreText = GameObject.Find("PlayerScore" + prefs.myID).GetComponent<Text>();
            laserScript.comboText = GameObject.Find("PlayerCombo" + prefs.myID).GetComponent<Text>();
            laserScript.scoreText.color = prefs.myColor;
            laserScript.comboText.color = prefs.myColor;

            playerList[i].obj.transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = prefs.myColor; //Laser color
            playerList[i].obj.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = prefs.myColor; //Trail renderer color
            playerList[i].obj.GetComponentInChildren<SpriteRenderer>().color = prefs.myColor; //Cannon color

            Debug.Log("player added, ID: " );
        }

        //Start the countdown to gameplay.
        StartCoroutine("CountDown");
    }

    //Brief grace period before diamonds start spawning.
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(startGameDelay);
        introText.SetActive(false);
        startGame = true;       //Once this is toggled, players have input.
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        GameObject topBarGroup = GameObject.Find("Top Bar Group");
        if (topBarGroup)
        {
            topBarGroup.SetActive(false);
        }
        //Iterate through each active player.
        for (int i = 0; i <= 3; i++)
        {
            //If the player is active.
            if (!playerList[i].active())
            {

                ///////////////////
                //Current gameOver handling, likely to be altered or removed in future commit.
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
        Text winnerText = GameObject.Find("WinnerText").GetComponent<Text>();
        winnerText.text = ("Player " + (winner + 1) + " Wins!");
        //Set the panel color to that of the winner.
        Image panel1 = GameObject.Find("GameOverPanel").GetComponent<Image>();
        panel1.color = new Color32(74, 68, 249, 255);
        ///////////////////
    }
    #endregion
    
    //Called from Cannon.cs.
    public void changeScene(int index) {
        SceneManager.LoadScene(index);
    }

    //Called from Laser.cs
    public void addScore(int pID, int score) {
        playerList[pID].setScore(score);
    }
}