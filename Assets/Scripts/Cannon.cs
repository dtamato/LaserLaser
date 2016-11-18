using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] int playerId;
	[SerializeField] float rotationSpeed = 2;
	[SerializeField] float maxBlastForce = 2200;
	[SerializeField] float maxAngleOffset = 70;

    //External References.
	Player rewiredPlayer;
	Laser storedLaser;
    GameManager gameManager;

    //Variables handling player rotation.
    float currentAngle;
	float angleOffset;
	float baseAngle;
	float minAngle;
	float maxAngle;
	int rotationModifier = 1;

    //Boolean Flags.
    bool inLobby = false;           //Player is in Lobby.
    bool playerIsActive = false;    //Player has joined the game (pressed A in Lobby).
    public bool gameOver = false;   //The game is over (input is removed).

    GameObject joinUI = null;
    Text scoreUI = null;
    Text comboUI = null;


	void Awake ()
    {
        rewiredPlayer = ReInput.players.GetPlayer (playerId);
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
        SetNewBaseAngle();
    }

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //If Lobby is the active scene.
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            inLobby = true;                                         //Set the player state to be in Lobby.
            joinUI = GameObject.Find("PlayerJoin" + playerId);      //Reference the Join UI cover.
        }

        //If Main Game is the active scene.
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            playerIsActive = gameManager.getPlayerState(playerId);      //Retrieve whether the player is active from the gameManager.
            
            //If the player isn't active, hide their cannon, their score text, and their combo text.
            if (!playerIsActive) {
                comboUI = GameObject.Find("PlayerCombo" + playerId).GetComponent<Text>();
                comboUI.text = "          ";
                scoreUI = GameObject.Find("PlayerScore" + playerId).GetComponent<Text>();
                scoreUI.text = "          ";
                gameObject.SetActive(false);
            }
            //If the player is active, remove their input temporarily. It's given back after the countdown.
            else {
                playerIsActive = false;
            }
        }
    }

	void Update ()
    {
        //Prevents player input until the game countdown has finished.
        if (!playerIsActive && gameManager.startGame == true)
            playerIsActive = true;
        
        //Enable Input from Player when they're mounted on a wall.
		if (storedLaser) {
            //If the player is in the lobby, and they aren't currently active.
            if (inLobby && !playerIsActive)
            {
                //If the player presses A.
                if (rewiredPlayer.GetButtonDown("Fire"))
                {
                    playerIsActive = true;                  //Activate the player.
                    gameManager.setPlayerState(playerId);   //Tell the gameManager the player is activated.
                    joinUI.SetActive(false);                //Take down the Join UI.
                }
            }

            //If the player is in the lobby, is currently active (has pressed A), and they press B.
            if (inLobby && playerIsActive && rewiredPlayer.GetButtonDown("Back"))
            {
                playerIsActive = false;                 //Deactivate the player.
                gameManager.setPlayerState(playerId);   //Tell the gameManager the player is deactivated.
                joinUI.SetActive(true);                 //Put the Join UI cover back up.
            }

            //If the player is not in the lobby (in the game), and the game is over (time ran out).
            if (!inLobby && gameOver)
            {
                playerIsActive = false;     //Removes movement and firing input from the player.
                //If the player presses start the game returns to the lobby.
                if (rewiredPlayer.GetButtonDown("StartGame"))
                {
                    gameManager.ReturnToLobby();
                }
            }

            //If a player who is active presses start, the game begins.
            if (inLobby && playerIsActive && rewiredPlayer.GetButtonDown("StartGame"))
            {
                SceneManager.LoadScene(1);
            }

            //Process gameplay Inputs.
            ProcessInputs ();
        }
	}

	void OnTriggerEnter2D(Collider2D other)
    {
        if (!storedLaser && other.CompareTag ("Player"))
            storedLaser = other.GetComponentInChildren<Laser> ();
	}

	void ProcessInputs ()
    {
        if (playerIsActive)
        {
            // Change direction of rotation if upside down
            if (this.transform.up == Vector3.up)
                rotationModifier = 1;
            else if (this.transform.up == Vector3.down)
                rotationModifier = -1;

            // Get controller joystick input
            if (rewiredPlayer.GetAxis("Horizontal") < 0)
            {
                //Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
                this.transform.Rotate(rotationSpeed * rotationModifier * Vector3.forward);
            }
            else if (rewiredPlayer.GetAxis("Horizontal") > 0)
            {
                //Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
                this.transform.Rotate(-rotationSpeed * rotationModifier * Vector3.forward);
            }

            // Restrict angle
            currentAngle = this.transform.rotation.eulerAngles.z;
            if (currentAngle < 0) { currentAngle += 360; }

            angleOffset = Mathf.Abs(currentAngle - baseAngle);

            //Debug.Log ("zAngle: " + zAngle);

            if (currentAngle >= maxAngle && currentAngle <= maxAngle + 5)
            {

                this.transform.rotation = Quaternion.Euler(0, 0, maxAngle);
            }
            else if (currentAngle <= minAngle && currentAngle >= minAngle - 5)
            {

                this.transform.rotation = Quaternion.Euler(0, 0, minAngle);
            }

            if (rewiredPlayer.GetButtonDown("Fire"))
            {
                //Debug.Log ("Fire!");
                StartCoroutine(TempDisableCollider());
                ShootOutPlayer(storedLaser.GetComponentInChildren<Rigidbody2D>());
                this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
                this.GetComponent<AudioSource>().Play();
            }
        }
    }

	IEnumerator TempDisableCollider () {

		this.GetComponent<Collider2D> ().enabled = false;

		yield return new WaitForSeconds (0.25f);

		this.GetComponent<Collider2D> ().enabled = true;
	}

	void ShootOutPlayer (Rigidbody2D playerRigidbody) {

		playerRigidbody.isKinematic = false;
		playerRigidbody.AddForce (maxBlastForce * this.transform.up);
		storedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
		storedLaser = null;
	}

	// Called from Laser.cs
	public void SetNewBaseAngle () {

		baseAngle = this.transform.rotation.eulerAngles.z;
		if (baseAngle < 0) { baseAngle %= 360; }

		minAngle = baseAngle - maxAngleOffset;
		if (minAngle < 0) { minAngle += 360; }

		maxAngle = baseAngle + maxAngleOffset;
		if (maxAngle < 0) { maxAngle += 360; }
	}

	public int GetPlayerID () {

		return playerId;
	}
}
