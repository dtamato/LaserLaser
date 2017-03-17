using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour
{
    //References.
    BaseGM gameManager;
    public Player rewiredPlayer;
    [SerializeField] Laser pairedLaser;     // Permanent reference to ball, manually assigned in Lobby scene and in player prefab.
    [SerializeField] Rigidbody2D laserRB;   // Reference to the ball's RB.
    public bool inFlight;                   // True if laser/ball is in flight, false if stored in cannon.
    public int playerId;
	[SerializeField] GameObject cannonExtension;

    //Control values.
    [SerializeField] float baseRotationSpeed;
    [SerializeField] float maxBlastForce;
    [SerializeField] float maxAngleOffset;
    
    //Rotation
    [SerializeField] float currentRotationSpeed;
    int rotationModifier = 1;
    float minRotationSpeed = 1.5f;
    float maxRotationSpeed = 5.0f;
    
    //Angles
    float currentAngle;
    float baseAngle;
    float minAngle;
    float maxAngle;
    public string testMode = "look in inspector";

	// Misc
	GameObject pauseMenu;
	bool openedPauseMenu = false;

    void Awake()
    {
        inFlight = false;
        laserRB = pairedLaser.GetComponent<Rigidbody2D>();

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>(); //to be used for test scenes. When game is published, this is to be removed
		            
		rewiredPlayer = ReInput.players.GetPlayer(playerId);

        if (maxAngleOffset < 0)
            maxAngleOffset *= -1;

        currentRotationSpeed = baseRotationSpeed;
        SetNewBaseAngle();

		if(GameObject.Find("Pause Menu")) {
			
			pauseMenu = GameObject.Find("Pause Menu").gameObject;
		}
    }

    void Update()
    {
        //Player can't input while in flight.
        if (!inFlight)
            ProcessInputs();
    }

    #region Inputs

    void ProcessInputs()
    {
		if(SceneManager.GetActiveScene().name.Contains("4")) {

			if(!openedPauseMenu && !pauseMenu.activeSelf) {

		        GetRotationInput();
		        RestrictAngle();
		        GetFireInput();
				CheckOpenPauseMenu();
			}
			else if(openedPauseMenu) {

				GetMenuInput();
			}
		}
		else {


			GetRotationInput();
			RestrictAngle();
			GetFireInput();
		}
    }

    void GetRotationInput()
    {
        // Get controller joystick input
        if (rewiredPlayer.GetAxisRaw("Horizontal") < 0)
        {
            currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			this.transform.Rotate(currentRotationSpeed * rotationModifier * Vector3.forward);
        }
        else if (rewiredPlayer.GetAxisRaw("Horizontal") > 0)
        {
            currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			this.transform.Rotate(-currentRotationSpeed * rotationModifier * Vector3.forward);
        }
        else if (rewiredPlayer.GetAxisRaw("Horizontal") == 0)
        {
            currentRotationSpeed = baseRotationSpeed;
        }
    }

    void RestrictAngle()
    {
		currentAngle = this.transform.rotation.eulerAngles.z;
        if (currentAngle < 0) { currentAngle += 360; }
        if (currentAngle >= maxAngle && currentAngle <= maxAngle + 5)
        {
			this.transform.rotation = Quaternion.Euler(0, 0, maxAngle);
        }
        else if (currentAngle <= minAngle && currentAngle >= minAngle - 5)
        {
			this.transform.rotation = Quaternion.Euler(0, 0, minAngle);
        }
    }

    void GetFireInput()
    {
        if (gameManager.gameOver == true && rewiredPlayer.GetButtonDown("StartGame")) {
            Debug.Log("changing to menu");
            gameManager.returnToMenu();
        }

		if(rewiredPlayer.GetButtonDown("Fire")) {
			
	        //When player fires, activate the laser and launch it with force.
			laserRB.bodyType = RigidbodyType2D.Dynamic;
			laserRB.GetComponent<Collider2D>().isTrigger = false;
			laserRB.AddForce(maxBlastForce * cannonExtension.transform.up);
			pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			pairedLaser.transform.GetComponent<TrailRenderer>().enabled = true;
			inFlight = true;
			this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
			this.GetComponent<AudioSource>().Play();
		}
    }

	void CheckOpenPauseMenu () {

		if(rewiredPlayer.GetButtonDown("StartGame") && !gameManager.gameOver) {

			pauseMenu.SetActive(true);
			pauseMenu.GetComponent<PauseMenu>().PauseGame(this.gameObject);
			openedPauseMenu = true;
		}
	}

	void GetMenuInput () {

		if(rewiredPlayer.GetAxisRaw("Horizontal") == 1) {

			pauseMenu.GetComponent<PauseMenu>().NextButton();
			Debug.Log("Next");
		}

		if(rewiredPlayer.GetButtonDown("StartGame")) {

			openedPauseMenu = false;
			pauseMenu.SetActive(false);
		}
	}

    #endregion

    #region Setters

    // Called from Laser.cs
    public void SetNewBaseAngle()
    {
        baseAngle = this.transform.rotation.eulerAngles.z;
        if (baseAngle < 0) { baseAngle %= 360; }
        minAngle = baseAngle - maxAngleOffset;
        if (minAngle < 0) { minAngle += 360; }
        maxAngle = baseAngle + maxAngleOffset;
        if (maxAngle < 0) { maxAngle += 360; }
        // Change rotation modifier if upside down
        rotationModifier = (this.transform.up == Vector3.down) ? -1 : 1;
    }

    //Called from CannonCustomization.cs in lobby when sensitivity is changed.
    public void ChangeRotationSpeed(float increment)
    {
        baseRotationSpeed += increment;
        baseRotationSpeed = Mathf.Clamp(baseRotationSpeed, minRotationSpeed, maxRotationSpeed);
    }

    //Called from BaseGM.cs in main game when player is instantiated.
    public void ApplyRotationSpeed(int sensitivity)
    {
        baseRotationSpeed = 1.0f + (sensitivity * 0.5f);
        currentRotationSpeed = baseRotationSpeed;
    }
    
    //Called from Slow.cs.
    public void ModifyRotationSpeed(float newSpeed)
    {
        baseRotationSpeed = newSpeed;
    }

	public void SetIsPaused (bool isPaused) {

		openedPauseMenu = false;
	}

	public void SetPauseMenu(GameObject menu) {

		pauseMenu = menu;
	}
    #endregion

    #region Getters
    public int GetPlayerID()
    {
        return playerId;
    }

    public float GetBaseSpeed()
    {
        return baseRotationSpeed;
    }
	public GameObject GetLaser () {

		return pairedLaser.gameObject;
	}
	public Player GetRewiredPlayer () {

		return rewiredPlayer;
	}
    #endregion
}