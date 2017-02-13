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

    //Control values.
    [SerializeField] float baseRotationSpeed;
    [SerializeField] float maxBlastForce;
    [SerializeField] float maxAngleOffset;
    
    //Rotation
    float currentRotationSpeed;
    int rotationModifier = 1;
    float minRotationSpeed = 2.0f;
    float maxRotationSpeed = 10.0f;
    
    //Angles
    float currentAngle;
    float baseAngle;
    float minAngle;
    float maxAngle;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        inFlight = false;
        laserRB = pairedLaser.GetComponent<Rigidbody2D>();

        //In the lobby this must be assigned. In the game scene the GM assigns it.
        if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex)
            rewiredPlayer = ReInput.players.GetPlayer(playerId);

        if (maxAngleOffset < 0)
            maxAngleOffset *= -1;

        currentRotationSpeed = baseRotationSpeed;
        SetNewBaseAngle();
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
        //In lobby, don't allow movement until player has joined.
        //In main game scene, once game is over don't allow movement.
		if ((SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex && this.GetComponent<CannonCustomization>().hasJoined) ||
			(SceneManager.GetActiveScene().buildIndex == gameManager.mainGameSceneIndex && gameManager.gameOver == false))
        {
            GetRotationInput();
            RestrictAngle();
        }

        //Always check for firing, also checks return to menu prompt on game over.
        GetFireInput();
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

        //When player fires, activate the laser and launch it with force.
        //Always enabled in the lobby. Only enabled in game after the start game countdown, but before game over.
        if (rewiredPlayer.GetButtonDown("Fire") && 
            (   SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex ||
                !gameManager.gameOver && gameManager.startGame))
        {
            //StartCoroutine(TempDisableCollider());
            laserRB.bodyType = RigidbodyType2D.Dynamic; 
            laserRB.AddForce(maxBlastForce * this.transform.up);
            pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			pairedLaser.transform.GetComponent<TrailRenderer> ().enabled = true;
            inFlight = true;
            this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
            this.GetComponent<AudioSource>().Play();
        }
    }

    /*
    IEnumerator TempDisableCollider()
    {
        this.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<Collider2D>().enabled = true;
    }
    */

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

    //Called from CannonCustomization.cs.
    public void ChangeRotationSpeed(float increment)
    {
        baseRotationSpeed += increment;
        baseRotationSpeed = Mathf.Clamp(baseRotationSpeed, minRotationSpeed, maxRotationSpeed);
    }
    
    //Called from Slow.cs.
    public void ModifyRotationSpeed(float newSpeed)
    {
        baseRotationSpeed = newSpeed;
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

    #endregion
}