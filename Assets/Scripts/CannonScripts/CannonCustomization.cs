using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]
[RequireComponent(typeof(Cannon))]
public class CannonCustomization : MonoBehaviour
{
    [SerializeField]
    float rotationSpeedIncrement;
    public bool isInLobby = true; //to check whether or not the player can change their color/rotation speed
    public bool hasJoined = false; //whether or not the player can control their cannon
    public bool canChange = false; //whether or not the player can change their color
    public int team;
    public bool inverted = false;
    public int sensitivity;
    public int colorIdx; //the player's position within the color array
    public Color myColor;
    public Color myTeamColor;
    public int myID;
    private Cannon cannon;
    public GameObject inputText;
    public string testMode = "look in inspector";

    // External references
    private BaseGM gameManager;
    private Player rewiredPlayer;
    private LobbyManager lobbyManager;
    void Awake()
    {
        cannon = this.GetComponentInChildren<Cannon>();
        rewiredPlayer = ReInput.players.GetPlayer(cannon.GetPlayerID());
        if (testMode != "debug") //to be removed when game is published. for test lobby purposes
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>(); //to be used for test scenes. When game is published, this is to be removed
        if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex)
            lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        //inputText = GameObject.Find("InputText" + myID);
        if (SceneManager.GetActiveScene().buildIndex == gameManager.mainGameSceneIndex)
            isInLobby = false;
        }
    }
    void Start()
    {
        if (testMode != "debug") //to be removed when game is published. for test lobby purposes
        {
            
        if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex) {
            myID = cannon.GetPlayerID();
            colorIdx = myID;
        }
        }
    }
    void Update()
    {
        if (isInLobby)
        {    //If the player is in the Lobby Scene.
            LobbyInputs();
        }
    }

    void LobbyInputs()
    {
        if (rewiredPlayer.GetButtonDown("Fire") && !hasJoined){    //If player presses 'A', and they are NOT currently in game.
            lobbyManager.UpdateColour(colorIdx, myID);                                  //Switching from grey to their default colour (Initially controlled via player ID).
            inputText.GetComponent<Text>().color = myColor;
            gameObject.GetComponent<Cannon>().enabled = true;                           //Letting the player control their cannon.
            GameObject.Find("JoinText" + myID).GetComponent<Text>().enabled = false;    //Hiding the "press 'A' to join text".
            lobbyManager.joinedPlayers++;
            hasJoined = true;
            if (lobbyManager.gameType == "FFA")
                team = myID + 1;
            else
                team = 0;
            gameManager.playerCount++;
            gameManager.playerJoin(myID, team, myColor,myTeamColor);    //Pass to GM.
        }

        if (rewiredPlayer.GetButtonDown("Back") && hasJoined)
        {     //If the player presses 'B', and they ARE currently in game.
            gameObject.GetComponent<Cannon>().enabled = false;                          //Revoking the ability for the user to control their cannon.
            lobbyManager.UnjoinColour(colorIdx, myID);                                  //Makes the player's color available to other players.
            lobbyManager.joinedPlayers--;
            hasJoined = false;
            gameManager.playerCount--;
            gameManager.playerLeave(myID);      //Pass to GM.
        }

        if (rewiredPlayer.GetButtonDown("RButt") && canChange)
        {   //If the player presses 'Right Bumper', and are currently in their team zone.
            colorIdx = lobbyManager.IncrementIndex(colorIdx);                           //Mathematical function to INCREMENT the player's index within the colour array.
            lobbyManager.UpdateColour(colorIdx, myID);                                  //Updating the player's colour.
            inputText.GetComponent<Text>().color = myColor;
            gameManager.setColor(myID, myColor);    //Pass to GM.
        }

        if (rewiredPlayer.GetButtonDown("LButt") && canChange)
        {    //If the player presses 'Left Bumper', and are currently in their team zone.
            colorIdx = lobbyManager.DecrementIndex(colorIdx);                           //Mathematical function to DECREMENT the player's index within the colour array.
            lobbyManager.UpdateColour(colorIdx, myID);
            inputText.GetComponent<Text>().color = myColor;
            gameManager.setColor(myID, myColor);    //Pass to GM.
        }

        if (rewiredPlayer.GetButtonDown("LTrigg"))
        {                //If the player presses 'Left Trigger'.
            lobbyManager.SwitchTeamMode(); //Switch the team mode in LobbyManager.cs.

            //Team info is passed to GM from the lobbyManager.
        }

        if (rewiredPlayer.GetButtonDown("RTrigg"))
        {                //If the player presses 'Right Trigger'.
            inverted = !inverted;
            gameManager.setInvert(myID, inverted);  //Pass to GM.
            if (inverted)
                inputText.GetComponent<Text>().text = "Inverted";
            else
                inputText.GetComponent<Text>().text = "Not Inverted";
            inputText.GetComponent<InputTextScript>().checkText();
            //Switch the player's inverted-ness. Needs to be implemented in Cannon.cs. Currently only passed as an empty setting.
        }

        if (rewiredPlayer.GetButtonDown("Setting"))
        {               //For future use, when the player will change their sensitivity, etc. from within the game's pause menu.
            Debug.Log("Opening Settings...");
        }

        if (rewiredPlayer.GetButtonDown("StartGame"))
        {             //If the player presses 'Start'.
            lobbyManager.StartGameCheck();
        }

        if (rewiredPlayer.GetButtonDown("IncreaseRotationSpeed"))
        { //If the player presses 'Up' on the D Pad.
            cannon.ChangeRotationSpeed(rotationSpeedIncrement);
            //Sensitivity is on a scale of 1-8. Corresponds to minRotSpeed of 2.0f, and maxRotSpeed of 10.0f.
            if (sensitivity < 8)
                sensitivity++;
            gameManager.setSensitivity(myID, sensitivity);  //Pass to GM.

            if (sensitivity == 8)
                inputText.GetComponent<Text>().text = "Sensitivity: MAX";
            else
                inputText.GetComponent<Text>().text = "Sensitivity: " + sensitivity;

            inputText.GetComponent<InputTextScript>().checkText();
        }

        if (rewiredPlayer.GetButtonDown("DecreaseRotationSpeed"))
        { //If the player presses 'Down' on the D Pad.
            cannon.ChangeRotationSpeed(-rotationSpeedIncrement);
            if (sensitivity > 1)
                sensitivity--;
            gameManager.setSensitivity(myID, sensitivity);  //Pass to GM.

            if (sensitivity == 1)
                inputText.GetComponent<Text>().text = "Sensitivity: MIN";
            else
                inputText.GetComponent<Text>().text = "Sensitivity: " + sensitivity;

            inputText.GetComponent<InputTextScript>().checkText();
        }
    }
}