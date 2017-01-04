using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Cannon))]
public class CannonCustomization : MonoBehaviour {

	[SerializeField] float rotationSpeedIncrement = 0.5f;
    [SerializeField] private Color myColour;

    private bool isInLobby = true;
    [SerializeField]
    private bool hasJoined = false;
    public bool canChange = false;

    public int colorIdx;
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
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID);
            hasJoined = true;
            GameObject.Find("JoinText" + myID).GetComponent<Text>().enabled = false;
        }

        else if (rewiredPlayer.GetButtonDown("Back") && hasJoined) //if the player presses 'B' to leave
        {
            hasJoined = false;
            //gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.grey; //graying out the player to show that it is inactive
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UnjoinColour(colorIdx, myID);
        }

        if (rewiredPlayer.GetButtonDown("RButt") && canChange) //can only change if the player is within the field, incrementing colour ->
        {
            colorIdx = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().IncrementIndex(colorIdx);
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID);
            myColour = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx]._color;
        }

        if (rewiredPlayer.GetButtonDown("LButt") && canChange) //decrementing colour <-
        {
            colorIdx = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().DecrementIndex(colorIdx);
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().UpdateColour(colorIdx, myID);
            myColour = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx]._color;
        }



        if (rewiredPlayer.GetButtonDown("Setting")) //for future use, when the player will change their sensitivity, etc.
        {
            Debug.Log("Opening Settings...");
        }


    }

    void ChangeRotationSpeed () {

		if (rewiredPlayer.GetButtonDown ("IncreaseRotationSpeed") || (rewiredPlayer.GetButton("IncreaseRotationSpeed"))) {

			cannon.ChangeRotationSpeed (rotationSpeedIncrement);
		    Debug.Log("Increasing rotation speed...");
		}
		else if (rewiredPlayer.GetButtonDown ("DecreaseRotationSpeed") || (rewiredPlayer.GetButton("DecreaseRotationSpeed"))) {

			cannon.ChangeRotationSpeed (-rotationSpeedIncrement);
		    Debug.Log("Decreasing rotation speed...");
		}
	}

}
