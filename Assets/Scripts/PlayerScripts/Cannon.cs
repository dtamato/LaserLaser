using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour
{
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

    //References.
    BaseGM gameManager;
    public Player rewiredPlayer;
    [SerializeField] Laser pairedLaser;     // Permanent reference to ball, manually assigned in Lobby scene and in player prefab.
    [SerializeField] Rigidbody2D laserRB;   // Reference to the ball's RB.
    public bool inFlight;                   // True if laser/ball is in flight, false if stored in cannon.
    public int playerId;

    //Control values.
    [SerializeField] float baseRotationSpeed;
    [SerializeField] float maxBlastForce;
    [SerializeField] float maxAngleOffset;
    
    //Rotation
    [SerializeField] float currentRotationSpeed;
    int rotationModifier = 1;
    float minRotationSpeed = 1.5f;
    float maxRotationSpeed = 5.0f;

    private Transform LTransform;
    private Transform RTransform;
    private Transform MTransform;
    private Vector2 LPoint;
    private Vector2 RPoint;
    private Vector2 MPoint;
    private const float cornerOffset = 0.6f;
    private Vector2 MOrigin;
    private int Layer_Mask;
    private float axisMod;

    [SerializeField] private bool collidingLeft;
    [SerializeField] private bool collidingRight;


    //Angles
    float currentAngle;
    float baseAngle;
    float minAngle;
    float maxAngle;

    //Pause
    GameObject pauseMenu;
    

    void Awake()
    {
        if (GameObject.Find("Pause Menu") != null)
        {
            pauseMenu = GameObject.Find("Pause Menu").gameObject;
        }
    }

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        gameManager.players[playerId] = this;

        joinText = GameObject.Find("JoinText" + playerId);
        inputText = GameObject.Find("InputText" + playerId);
        laserRB = pairedLaser.GetComponent<Rigidbody2D>();
        rewiredPlayer = ReInput.players.GetPlayer(playerId);

        inFlight = false;
        sensitivity = 5;

        LTransform = transform.Find("LPoint").transform;
        RTransform = transform.Find("RPoint").transform;
        MTransform = transform.Find("MPoint").transform;
        LPoint = new Vector2(LTransform.position.x,LTransform.position.y);
        RPoint = new Vector2(RTransform.position.x, RTransform.position.y);
        MPoint = new Vector2(MTransform.position.x, MTransform.position.y);
        //MOrigin = new Vector2(MPoint.transform.position.x,MPoint.transform.position.y);


        Layer_Mask = LayerMask.GetMask("Boundary");

    	colorIdx = playerId;
        gameManager.UpdateColour(colorIdx, playerId);
        inputText.GetComponent<Text>().color = myColor; //to be changed when control is fixed
		joinText.GetComponent<Text>().text = "Press Start When Ready!";


        if (gameManager.gameMode == "FFA")
            team = playerId + 1;
        else
            team = 0;

       

        //Setup for rotation.
        if (maxAngleOffset < 0)
            maxAngleOffset *= -1;
        currentRotationSpeed = baseRotationSpeed;
        SetNewBaseAngle();
    }

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
                if (gameManager.GetPaused() == false && !pauseMenu.activeSelf)
                {
                    StandardInputs();
                    CheckOpenPauseMenu();
                }
                else if (gameManager.GetPaused())
                {
                    GetMenuInput();
                }

                else
                {
                    StandardInputs();
                }
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
            GetRotationInput();
            RestrictAngle();
            GetFireInput();
        }
    }

    #region Inputs
    //
    void GetRotationInput()
    {
        axisMod = rewiredPlayer.GetAxis("Horizontal");
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
        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
        this.transform.Rotate(currentRotationSpeed * rotationModifier * Vector3.forward * -axisMod);
    }

    void CheckOpenPauseMenu()
    {

        if (rewiredPlayer.GetButtonDown("StartGame") && gameManager.getState() != BaseGM.GAMESTATE.POSTGAME)
        {
            
            pauseMenu.SetActive(true);
            pauseMenu.GetComponent<PauseMenu>().PauseGame(this.gameObject);
            gameManager.SetPaused(true);
            gameManager.SetPlayerPauseId(this.playerId);
            gameManager.DisablePlayerControllers(playerId);
        }
    }



    void GetMenuInput()
    {

        if (rewiredPlayer.GetAxisRaw("Vertical") == 1 && playerId == gameManager.GetPlayerPauseId())
        {
            pauseMenu.GetComponent<PauseMenu>().NextButton();
            Debug.Log("Next");
        }

        if (rewiredPlayer.GetButtonDown("StartGame"))
        {

            gameManager.EnablePlayerControllers();
            gameManager.SetPaused(false);
            gameManager.SetPlayerPauseId(-1);
            pauseMenu.SetActive(false);
        }
    }

    //
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

    //
    void GetFireInput()
    {
         if (rewiredPlayer.GetButtonDown("Fire"))
         {
	         laserRB.bodyType = RigidbodyType2D.Dynamic;
			 laserRB.GetComponent<Collider2D>().isTrigger = false;
	         laserRB.AddForce(maxBlastForce * this.transform.up);
	         pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
	         pairedLaser.transform.GetComponent<TrailRenderer>().enabled = true;
	         inFlight = true;
	         this.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.15f);
	         this.GetComponent<AudioSource>().Play();
         }
    }

    //customization inputs
    void PregameInputs()
    {

        //*********************************HERE******************************


        if (rewiredPlayer.GetButtonDown("Back"))        //Player presses B.
        {
            if (playerReady)
            {
                playerReady = false;
                gameManager.readyPlayers--;
            }
            else
            {
                spawnPoint.GetComponent<SpawnListener>().taken = false;
                gameManager._colorlist[colorIdx].isAvailable = true;
                joinText.GetComponent<Text>().text = "Press 'A' to Join";
                gameManager.playerCount--;
                Destroy(this.gameObject);
            }
        }


        //*********************************HERE******************************

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
            if (sensitivity >= 8)
            {
                inputText.GetComponent<Text>().text = "Sensitivity: MAX";
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
            if (sensitivity >= 1)
            {
                sensitivity--;
                ChangeRotationSpeed(-rotationSpeedIncrement);
            }
            else
            {
                inputText.GetComponent<Text>().text = "Sensitivity: MIN";
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

    public void SetID(int newId)
    {
        playerId = newId;
    }

    public void SetIsPaused(bool isPaused)
    {

        gameManager.SetPaused(isPaused);
    }

    public void SetPauseMenu(GameObject menu)
    {

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