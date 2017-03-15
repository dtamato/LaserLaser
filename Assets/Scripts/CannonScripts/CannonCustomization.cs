using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
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

            if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex)
            {
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

}

    */