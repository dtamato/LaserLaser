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

    //Lobby Handling Variables.
    bool inLobby = false;
    bool playerIsActive = false;
    GameObject joinUI = null;
    Text scoreUI = null;
    public bool gameOver = false;


	void Awake ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rewiredPlayer = ReInput.players.GetPlayer (playerId);
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
		SetNewBaseAngle ();

        //If Lobby is the active scene.
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            inLobby = true;
            joinUI = GameObject.Find("PlayerJoin" + playerId);
        }
        //If Main Game is the active scene.
        else if (SceneManager.GetActiveScene().buildIndex == 1) {
            playerIsActive = gameManager.getPlayerState(playerId);
            //If the player isn't active, remove their score UI and the player.
            if (!playerIsActive) {
                scoreUI = GameObject.Find("PlayerScore" + playerId).GetComponent<Text>();
                scoreUI.text = "          ";
                gameObject.SetActive(false);
            }
        }
    }

	void Update ()
    {
        //Enable Input from Player.
		if (storedLaser)
			ProcessInputs ();
	}

	void OnTriggerEnter2D(Collider2D other)
    {
        if (!storedLaser && other.CompareTag ("Player"))
            storedLaser = other.GetComponentInChildren<Laser> ();
	}

	void ProcessInputs ()
    {
        if (!playerIsActive && inLobby) {
            if (rewiredPlayer.GetButtonDown("Fire")) {
                playerIsActive = true;
                gameManager.setPlayerState(playerId);
                joinUI.SetActive(false);
            }
        }

        //Once game over is triggered take away control from the player.
        //Only allow start to be pressed to return to lobby.
        if (gameOver && !inLobby) {
            playerIsActive = false;
            if (rewiredPlayer.GetButtonDown("StartGame"))
            {
                SceneManager.LoadScene(0);
            }
        }


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

            //If the player presses B while in the lobby, the player is removed from play.
            if (rewiredPlayer.GetButtonDown("Back") && inLobby)
            {
                playerIsActive = false;
                gameManager.setPlayerState(playerId);
                joinUI.SetActive(true);
            }

            //If the player 1 presses Start while in the lobby, the game begins with whatever players are joined.
            if (rewiredPlayer.GetButtonDown("StartGame") && inLobby && playerId == 0) {
                SceneManager.LoadScene(1);
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
