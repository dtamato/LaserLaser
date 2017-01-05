using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Cannon))]
public class CannonCustomization : MonoBehaviour {

	[SerializeField] float rotationSpeedIncrement = 0.5f;
    [SerializeField] private Color myColour; //will be used by the gameplay manager to control the user's colour upon initialization

    private bool isInLobby = true; //to check whether or not the player can change their colour/rotation speed
    [SerializeField] private bool hasJoined = false; //whether or not the player can control their cannon
    public bool canChange = false; //whether or not the player can change their colour

    public int colorIdx; //the player's position within the colour array
    private int myID;

    Cannon cannon;

	// External references
	GameControllerParent gameController;
	Player rewiredPlayer;

	void Awake () {

		cannon = this.GetComponentInChildren<Cannon> ();
		rewiredPlayer = ReInput.players.GetPlayer (cannon.GetPlayerID ());
		//gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameControllerParent> ();
	}

    void Start()
    {
        myID = cannon.GetPlayerID();
        colorIdx = myID;
    }

	void Update () {

		ProcessInputs ();
	}

	void ProcessInputs () {

	    if (isInLobby)
	    {
	        ActivationCheck();
		    ChangeRotationSpeed ();
            LobbyInputs();
        }

	}

    void ActivationCheck()
    {
        if (hasJoined)
            gameObject.GetComponent<Cannon>().enabled = true;
        else
            gameObject.GetComponent<Cannon>().enabled = false;
    }

    void LobbyInputs()
    {
        if (rewiredPlayer.GetButtonDown("Fire") && !hasJoined) //Press 'A' to join
        {
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID); //switching from grey to their default colour (Initially controlled via player ID)
            hasJoined = true; //Letting the player control their cannon
            GameObject.Find("JoinText" + myID).GetComponent<Text>().enabled = false; //Hiding the "press 'A' to join text"
        }

        else if (rewiredPlayer.GetButtonDown("Back") && hasJoined) //if the player presses 'B' to leave
        {
            hasJoined = false; //Revoking the ability for the user to control their cannon
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UnjoinColour(colorIdx, myID);
        }

        if (rewiredPlayer.GetButtonDown("RButt") && canChange) //can only change if the player is within the field, incrementing colour ->
        {
            colorIdx = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().IncrementIndex(colorIdx); //Mathematical function to update the player's index within the colour array
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID); //updating the player's colour
            myColour = GameObject.Find("LobbyManager").GetComponent<LobbyManager>()._colorlist[colorIdx]._color; //setting the myColour variable so be used by the GameManager for gameplay initialization
        }

        if (rewiredPlayer.GetButtonDown("LButt") && canChange) //decrementing colour <-
        {
            colorIdx = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().DecrementIndex(colorIdx);
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID);
            myColour = GameObject.Find("LobbyManager").GetComponent<LobbyManager>()._colorlist[colorIdx]._color;
        }



        if (rewiredPlayer.GetButtonDown("Setting")) //for future use, when the player will change their sensitivity, etc. from within the game
        {
            Debug.Log("Opening Settings...");
        }


    }

    void ChangeRotationSpeed () {

		if (rewiredPlayer.GetButtonDown ("IncreaseRotationSpeed") || (rewiredPlayer.GetButton("IncreaseRotationSpeed"))) {

			cannon.ChangeRotationSpeed (rotationSpeedIncrement);
		    //Debug.Log("Increasing rotation speed...");
		}
		else if (rewiredPlayer.GetButtonDown ("DecreaseRotationSpeed") || (rewiredPlayer.GetButton("DecreaseRotationSpeed"))) {

			cannon.ChangeRotationSpeed (-rotationSpeedIncrement);
		    //Debug.Log("Decreasing rotation speed...");
		}
	}

}
