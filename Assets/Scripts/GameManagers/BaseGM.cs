using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

//Used to assign color's to players when they press RB or LB.
public class ColorList
{
    public bool isAvailable = true;
    public Color _color;

    public ColorList(bool avail, Color col)
    {
        isAvailable = avail;
        _color = col;
    }
}

public class BaseGM : MonoBehaviour
{
    #region Variables
    
    //Scene build index integers.
    public int menuSceneIndex;
    //public int LobbySceneIndex;
    public int mainGameSceneIndex;
    public int gameOverSceneIndex;
   
    //External References.
    public static BaseGM instance = null;
    public GameObject playerObj;
    public List<Cannon> players;
    protected List<GameObject> spawns;
    protected GameObject[] activePlayersArray;

    //HUD Elements.
    protected List<Text> HUDText;
    protected List<Text> joinText;
    protected List<Text> inputText;
    protected GameObject readyText;
    protected Text joinCountdownText;
    protected GameObject gameOverPanel;
    protected GameObject whiteBorder;

    //State management.
    public enum GAMESTATE {SETUP, PREGAME, COUNTDOWN, INGAME, POSTGAME };
    protected GAMESTATE state;  //Set first to pregame in initializeGame().
    public string gameMode;

    //Color management.
    public ColorList[] _colorlist = new ColorList[7];
    void FFAColourList() //The available colours for the FFA lobby
    {
        _colorlist[0] = new ColorList(false, new Color(252 / 255f, 0, 1));
        _colorlist[1] = new ColorList(false, new Color(156 / 255f, 0, 1));
        _colorlist[2] = new ColorList(false, new Color(12 / 255f, 0, 1));
        _colorlist[3] = new ColorList(false, new Color(79 / 255f, 1, 223 / 255f));
        _colorlist[4] = new ColorList(true, new Color(89 / 255f, 254 / 255f, 50 / 255f));
        _colorlist[5] = new ColorList(true, new Color(1, 168 / 255f, 0));
        _colorlist[6] = new ColorList(true, new Color(1, 0, 0));
    }
    
    //Timer and score metrics.
    public float joinGameDelay;
    public float startGameDelay;
    public int playerCount = 0;
    public int readyPlayers = 0;
    public List<int> playerScores;
    public int team1Score, team2Score;  //Only used in TB mode.

    #endregion

    #region Player Prefs (Currently Unused)

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
        [SerializeField] private Color teamColor;

        //Constructor sets defaults for player settings.
        public PlayerDef()
        {
            ID = -1;
            isActive = false;
            sensitivity = 5;
            inverted = false;
            team = -1;
            color = Color.grey;
            teamColor = new Color(0.8f, 0.8f, 0.8f, 0f);
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

        public void setTeamColor(Color value)
        {
            teamColor = value;
        }

        //List of getters for private variables.
        public int getID(){
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
        public Color getTeamColor() {
            return teamColor;
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
            teamColor = new Color(0.8f,0.8f,0.8f,0f);
        }
    }

    #endregion

    #region Player Preferences (Public Accessors)

    /*
    //Called from Cannon.cs when a player enters the lobby.
    public void playerJoin(int pID, int teamNo, Color color, Color teamColor)
    {
        playerList[pID].setID(pID);
        playerList[pID].setTeam(teamNo);
        playerList[pID].setColor(color);
        playerList[pID].setTeamColor(teamColor);
        playerList[pID].setActive(true);
    }
    
    //The following getters are called from Cannon.cs to access the protected PlayerDefs class.
    public void playerLeave(int pID) {
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
    public void setID(int pID) {
        playerList[pID].setID(pID);
    }
    public void setTeamColour(int pId, Color color) {
        playerList[pId].setTeamColor(color);
    }
    */

    #endregion

    /*
    public void IncrementReadyPlayers()
    {
        readyPlayers++;
    }

    public void DecrementReadyPlayers()
    {
        readyPlayers--;
    }
    */

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
        joinText = new List<Text>(4);
        inputText = new List<Text>(4);
        spawns = new List<GameObject>(4);
        playerScores = new List<int>(4);

        //Initialize HUD, Spawns, Scores, and Colourlist.
        for (int i = 0; i <= 3; i++)
        {
            HUDText.Add(null);
            joinText.Add(null);
            inputText.Add(null);
            spawns.Add(null);
            players.Add(null);
            playerScores.Add(0);
        }

        //This will need to be changed when the GM is instantiated properly in the menu and carried into the game scene.
        state = GAMESTATE.SETUP;
        FFAColourList();
    }

    #region MainGame Scene

    //Called when entering game scene, initializes players, HUD and timer.
    protected void initializeGame()
    {
        //Reference members of HUD and Spawns.
        for (int i = 0; i <= 3; i++)
        {
            HUDText[i] = GameObject.Find("PlayerScore" + i).GetComponent<Text>();
            joinText[i] = GameObject.Find("JoinText" + i).GetComponent<Text>();
            inputText[i] = GameObject.Find("InputText" + i).GetComponent<Text>();
            spawns[i] = GameObject.Find("SP" + i);
        }

        //Reference all UI elements, and the 1st player object.
        readyText = GameObject.Find("ReadyText");
        readyText.SetActive(false);
        joinCountdownText = GameObject.Find("JoinCountdownText").GetComponent<Text>();
        whiteBorder = GameObject.Find ("White Border");
        gameOverPanel = GameObject.Find("GameOverPanel");
        gameOverPanel.SetActive(false);

        Debug.Log("initialize ran.");
        //FillActivePlayersArray ();
    }

    //Brief grace period before diamonds start spawning.
    protected IEnumerator CountDown()
    {
        yield return new WaitForSeconds(startGameDelay);
        SetState(GAMESTATE.INGAME);
        Debug.Log(state);
        readyText.SetActive(false);
    }

    //Called from each subclass GM.
    public void GameOver()
    {
        state = GAMESTATE.POSTGAME;
        Debug.Log(state);
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
                int score = playerScores[i];
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
            GameObject.Find("GameOverPanel").GetComponent<Image>().color = players[winner].GetColor();
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
        playerScores[pID] += score;
		UpdateWhiteBorderFFA ();
        HUDText[pID].text = "P" + (pID + 1) + "- " + score.ToString("00");
    }

    //Called to update the border of the arena with the color of the leading player.
	void UpdateWhiteBorderFFA ()
    {
		int winningScore = -1;
		int winningPlayerIndex = -1;

		for (int i = 0; i < 4; i++)
        {
			if (playerScores[i] > winningScore)
            {
                winningScore = playerScores[i];
                winningPlayerIndex = i;
                whiteBorder.GetComponent<SpriteRenderer>().color = players[i].GetColor();
			}
			else if (playerScores[i] == winningScore)
            {
				whiteBorder.GetComponent<SpriteRenderer> ().color = Color.white;
			}
		}
	}

    //Called to update the border in team mode.
    void UpdateWhiteBorderTB()
    {
        if (team1Score > team2Score)
        {
            whiteBorder.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (team2Score > team1Score)
        {
            whiteBorder.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            whiteBorder.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //Called to update team scores.
    public void addToTeamScore(bool isTeam1)
    {
		if (isTeam1)
        {
			team1Score++;
		}
		else
        {
			team2Score++;
		}

		UpdateWhiteBorderTB ();
	}

    //Called initially to make player objects available to powerups.
    /*
    void FillActivePlayersArray()
    {

        List<GameObject> activePlayersList = new List<GameObject>();

        for (int i = 0; i < playerList.Count; i++)
        {

            if (playerList[i].obj != null)
            {

                activePlayersList.Add(playerList[i].obj);
            }
        }

        activePlayersArray = activePlayersList.ToArray();
    }
    */

    //Called from Slow.cs and Paralysis.cs, as well as possible future powerups.
    public GameObject[] GetActivePlayers () {

		return activePlayersArray;
	}
    
    //From LobbyManager
    public int IncrementIndex(int idx) //if the player wants to change their colour forward ->
    {
        _colorlist[idx].isAvailable = true; //accessing the array of available colours contained within ColorManager
        do
        {
            idx++;
            idx %= 7;
        } while (!_colorlist[idx].isAvailable);
        return idx;
    }

    public int DecrementIndex(int idx) //if the player wants to change their colour backward <-
    {
        _colorlist[idx].isAvailable = true;
        do
        {
            idx = (idx - 1) % 7;
            idx = idx < 0 ? idx + 7 : idx; //is check 1 true? if yes, use check 2 (wraps around back to the end of the array when you're decrementing past the first element)
        } while (!_colorlist[idx].isAvailable);
        return idx;
    }

    //We'll have to fix this.
    public void UpdateColour(int cIdx, int pId) //if the player tries to change their colour, it will be updated here. Also, if the player joins too. 
    {
        players[pId].transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Laser colour
        players[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = _colorlist[cIdx]._color; //Trail renderer 
        players[pId].GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Cannon colour
        _colorlist[cIdx].isAvailable = false; //making sure other players cannot use the same colour
        players[pId].myColor = _colorlist[cIdx]._color;   //Update Color variable, to be passed to the GM.
        //players[pId].GetComponent<Cannon>().inputText.GetComponent<Text>().color = _colorlist[cIdx]._color;
        players[pId].GetComponent<Cannon>().myColor = _colorlist[cIdx]._color; ;
    }

    //Called from cannon.cs.
    public GAMESTATE getState()
    {
        return state;
    }

    public void SetState(GAMESTATE test)
    {
        state = test;
    }

}