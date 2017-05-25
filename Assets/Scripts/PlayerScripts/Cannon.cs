using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum Direction { Up, Down, Left, Right };

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour
{
	#region Variables

    //from customization script
    private int colorIdx; //the player's position within the color array
    private float rotationSpeedIncrement = 1;
    public GameObject inputText;
	public GameObject joinText;
    public Color myColor;
    public Color myTeamColor;
    public int team;
    public int sensitivity;
    private bool playerReady = false;
    public GameObject spawnPoint;   //set from spawnpoint script.

    //References
    BaseGM gameManager;
    public Player rewiredPlayer;
    [SerializeField] Laser pairedLaser;     // Permanent reference to ball, manually assigned in Lobby scene and in player prefab.
    [SerializeField] Rigidbody2D laserRB;   // Reference to the ball's RB.    
    public int playerId;   
    
    //Rotation
	[SerializeField] float rotationSpeed;
	Direction direction;
	int rotationModifier = 1;
    float minRotationSpeed = 1.0f;
    float maxRotationSpeed = 5.0f;

	[Header("Shooting")]
	[SerializeField] float minBlastForce;
	[SerializeField] float maxBlastForce;
	[SerializeField] float maxAngleOffset;
	[SerializeField] Image blastFill;
	float blastForce;
	float chargeTime;
	float maxChargeTime = 0.3f;
	public bool inFlight;                   // True if laser/ball is in flight, false if stored in cannon.

    private Transform LTransform;
    private Transform RTransform;
    private Transform MTransform;
    private Vector2 LPoint;
    private Vector2 RPoint;
    private Vector2 MPoint;
    private Vector2 MOrigin;
    private int Layer_Mask;
    private float axisMod;
	private bool collidingLeft;
	private bool collidingRight;

    //Angles
    float currentAngle;
    float baseAngle;
    float minAngle;
    float maxAngle;

	#endregion

	#region Initialization
    void Start()
    {
		InitializeReferences ();
		InitializeParameters ();
		GetNextAvailableColor ();
    }

	void InitializeReferences () {

		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
		gameManager.players[playerId] = this;

		rewiredPlayer = ReInput.players.GetPlayer(playerId);
		laserRB = pairedLaser.GetComponent<Rigidbody2D>();

		joinText = GameObject.Find("JoinText" + playerId);
		joinText.GetComponent<Text>().text = "Press Start When Ready!";

		inputText = GameObject.Find("InputText" + playerId);

		Layer_Mask = LayerMask.GetMask("Boundary");

		LTransform = transform.Find("LPoint").transform;
		RTransform = transform.Find("RPoint").transform;
		MTransform = transform.Find("MPoint").transform;
		LPoint = new Vector2(LTransform.position.x,LTransform.position.y);
		RPoint = new Vector2(RTransform.position.x, RTransform.position.y);
		MPoint = new Vector2(MTransform.position.x, MTransform.position.y);

		blastFill.gameObject.SetActive (false);
	}

	void InitializeParameters () {

		inFlight = false;

		team = (gameManager.gameMode == "FFA") ? playerId + 1 : 0;
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
		SetNewBaseAngle();

		sensitivity = 5;
		SetRotationSpeed (5f);
	}

	void GetNextAvailableColor () {

		int nextAvailableColorIndex = playerId;

		while(gameManager._colorlist[nextAvailableColorIndex].isAvailable == false) {

			nextAvailableColorIndex++;
			nextAvailableColorIndex %= gameManager._colorlist.Length;
		}

		colorIdx = nextAvailableColorIndex;

		gameManager.UpdateColour(colorIdx, playerId);
		inputText.GetComponent<Text>().color = myColor;
	}

	#endregion

    void Update()
    {
        switch (gameManager.getState())
        {
            case (BaseGM.GAMESTATE.PREGAME):
                PregameInputs();
                StandardInputs();
                break;

            //In countdown player can only rotate, no firing.
            case (BaseGM.GAMESTATE.COUNTDOWN):
                StandardInputs();
                break;

            case (BaseGM.GAMESTATE.INGAME):
				StandardInputs();
                break;

            case (BaseGM.GAMESTATE.POSTGAME):

                if (gameManager.getState() == BaseGM.GAMESTATE.POSTGAME && rewiredPlayer.GetButtonDown("StartGame"))
                {
                    Debug.Log("changing to menu");
                    gameManager.returnToMenu();
                }
                break;
        }
    }

    void StandardInputs()
    {
        if (!inFlight)
        {
            ProcessRotation();
            RestrictAngle();
            GetFireInput();
        }
    }

    #region Inputs
    //
    void ProcessRotation() {
	
		GetControllerInput ();
		CheckForWallCollision ();

        rotationSpeed = Mathf.Clamp(rotationSpeed, minRotationSpeed, maxRotationSpeed);
        this.transform.Rotate(rotationSpeed * rotationModifier * Vector3.forward * -axisMod);
    }

	void GetControllerInput () {

		float horizontalInput = rewiredPlayer.GetAxis ("Horizontal");
		float verticalInput = rewiredPlayer.GetAxis ("Vertical");

		if (Mathf.Abs (horizontalInput) > Mathf.Abs (verticalInput)) {
			
			verticalInput = 0;
			axisMod = (direction == Direction.Right) ? -horizontalInput : horizontalInput;
		}
		else if (Mathf.Abs (verticalInput) > Mathf.Abs (horizontalInput)) {

			horizontalInput = 0;
			axisMod = (direction == Direction.Down) ? -verticalInput : verticalInput;
		}
		else {

			axisMod = 0;
		}
	}

	void CheckForWallCollision () {

		LPoint = new Vector2(LTransform.position.x, LTransform.position.y); //making a Vector2 out of the object's transforms
		RPoint = new Vector2(RTransform.position.x, RTransform.position.y);
		MPoint = new Vector2(MTransform.position.x, MTransform.position.y);

		collidingLeft = Physics2D.Linecast(MPoint, LPoint, Layer_Mask);
		collidingRight = Physics2D.Linecast(MPoint ,RPoint, Layer_Mask);

		if (collidingLeft)
		{
			if (rotationModifier == -1)
			{
				axisMod = Mathf.Clamp(axisMod, -1f, 0f);
			}
			else
			{
				axisMod = Mathf.Clamp(axisMod, 0f, 1f);
			}
		}

		if (collidingRight)
		{
			if (rotationModifier == -1)
			{
				axisMod = Mathf.Clamp(axisMod, 0f, 1f);
			}
			else
			{
				axisMod = Mathf.Clamp(axisMod, -1f, 0f);
			}

		}
	}
		
    void RestrictAngle()
    {
        currentAngle = this.transform.rotation.eulerAngles.z;
        if (currentAngle < 0) { currentAngle += 360; }

        if (currentAngle >= maxAngle && currentAngle <= maxAngle + 10)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, maxAngle);
        }
        else if (currentAngle <= minAngle && currentAngle >= minAngle - 10)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, minAngle);
        }
    }
		
    void GetFireInput()
    {
		if (inFlight)
			return;

		if (rewiredPlayer.GetButtonDown ("Fire")) {

			blastForce = 0;
			chargeTime = 0;
			blastFill.fillAmount = 0;
			blastFill.gameObject.SetActive (true);
		}
		else if (rewiredPlayer.GetButton ("Fire") && !inFlight) {
			
			chargeTime += Time.deltaTime;
			chargeTime = Mathf.Clamp (chargeTime, 0, maxChargeTime);

			float unclampedBlastForce = (chargeTime / maxChargeTime) * maxBlastForce;
			blastForce = Mathf.Clamp (unclampedBlastForce, minBlastForce, maxBlastForce);
			if (blastFill.gameObject.activeSelf == false) { blastFill.gameObject.SetActive (true); }
			blastFill.fillAmount = chargeTime / maxChargeTime;
		}
		else if (rewiredPlayer.GetButtonUp("Fire") && blastFill.gameObject.activeSelf)
		{
			laserRB.bodyType = RigidbodyType2D.Dynamic;
			laserRB.GetComponent<Collider2D>().isTrigger = false;
			laserRB.AddForce(blastForce * this.transform.up);
			pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			pairedLaser.transform.GetComponent<TrailRenderer>().enabled = true;
			inFlight = true;
			this.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.15f);
			this.GetComponent<AudioSource>().Play();

			blastForce = 0;
			chargeTime = 0;
			blastFill.fillAmount = 0;
			blastFill.gameObject.SetActive(false);
		}
    }

    //customization inputs
    void PregameInputs()
    {
        if (rewiredPlayer.GetButtonDown("RButt"))       //Player presses RB.
        { 
            colorIdx = gameManager.IncrementIndex(colorIdx);
            gameManager.UpdateColour(colorIdx, playerId);
        }

        if (rewiredPlayer.GetButtonDown("LButt"))       //Player presses LB.
        {
            colorIdx = gameManager.DecrementIndex(colorIdx);
            gameManager.UpdateColour(colorIdx, playerId);
        }

        if (rewiredPlayer.GetButtonDown("StartGame"))   //Player presses Start.
        {
            if (!playerReady)
            {
                playerReady = true;
                gameManager.readyPlayers++;
                joinText.GetComponent<Text>().text = "Ready!";
            }
            else
            {
                playerReady = false;
                gameManager.readyPlayers--;
                joinText.GetComponent<Text>().text = "Press Start When Ready!";
            }
        }

        if (rewiredPlayer.GetButtonDown("IncreaseRotationSpeed"))      //Player presses UpD.
        {
            
            //Sensitivity is on a scale of 1-8. Corresponds to minRotSpeed of 2.0f, and maxRotSpeed of 10.0f.
            if (sensitivity >= 5)
            {
                inputText.GetComponent<Text>().text = "Sensitivity: 5";
            }
            else
            {
                ChangeRotationSpeed(rotationSpeedIncrement);
                sensitivity++;
            }
              
            inputText.GetComponent<Text>().text = "Sensitivity: " + sensitivity;
            inputText.GetComponent<InputTextScript>().checkText();
        }

        if (rewiredPlayer.GetButtonDown("DecreaseRotationSpeed"))         //Player presses DownD.
        {
            if (sensitivity > 1)
            {
                sensitivity--;
                ChangeRotationSpeed(-rotationSpeedIncrement);
            }

            inputText.GetComponent<Text>().text = "Sensitivity: " + sensitivity;
            inputText.GetComponent<InputTextScript>().checkText();
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
    }

	public void ChangeRotationDirection () {

		if (this.transform.up == Vector3.up) {
			
			direction = Direction.Up;
			rotationModifier = 1;
		}
		else if (this.transform.up == Vector3.down) {

			direction = Direction.Down;
			rotationModifier = -1;
		}
		else if (this.transform.up == Vector3.left) {

			direction = Direction.Left;
			rotationModifier = 1;
		}
		else if (this.transform.up == Vector3.right) {

			direction = Direction.Right;
			rotationModifier = -1;
		}
	}

    //Called from CannonCustomization.cs in lobby when sensitivity is changed.
    public void ChangeRotationSpeed(float increment)
    {
        rotationSpeed += increment;
		rotationSpeed = Mathf.Clamp(rotationSpeed, minRotationSpeed, maxRotationSpeed);
    }
    
    public void SetRotationSpeed(float newSpeed)
    {
		rotationSpeed = newSpeed;
    }

    public void SetID(int newId)
    {
        playerId = newId;
    }

    #endregion

    #region Getters

    public int GetPlayerID()
    {
        return playerId;
    }

    public float GetRotationSpeed()
    {
		return rotationSpeed;
    }

	public GameObject GetLaser ()
    {
		return pairedLaser.gameObject;
	}

    public Color GetColor()
    {
        return myColor;
    }

    public Player GetRewiredPlayer()
    {
        return rewiredPlayer;
    }
    #endregion
}