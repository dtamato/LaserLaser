using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour
{
    BaseGM gameManager;
    public bool inFlight;

    public int playerId;
    [SerializeField] float baseRotationSpeed = 2;
    [SerializeField] float maxBlastForce = 2200;
    [SerializeField] float maxAngleOffset = 70;
    
    // Rotation
    float currentRotationSpeed;
    int rotationModifier = 1;
    float minRotationSpeed = 2f;
    float maxRotationSpeed = 10.0f;
    
    // Angles
    float currentAngle;
    float angleOffset;
    float baseAngle;
    float minAngle;
    float maxAngle;
    public Player rewiredPlayer;
    [SerializeField] Laser pairedLaser; // Permanent reference to ball
    [SerializeField] Laser storedLaser; // Reference used to check if can fire
    

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        inFlight = false;

        if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex)
        {
            rewiredPlayer = ReInput.players.GetPlayer(playerId);
            
            Debug.Log("Rewired ID join: " + rewiredPlayer.id);
        }
        currentRotationSpeed = baseRotationSpeed;
        if (maxAngleOffset < 0)
        {
            maxAngleOffset *= -1;
        }
        SetNewBaseAngle();
    }

    void Update()
    {
        //If in the lobby, process inputs. If in the game, check the countdown is finished as well.
        if (!inFlight)
        {
            if (SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex)
                ProcessInputs();
            else if (SceneManager.GetActiveScene().buildIndex == gameManager.mainGameSceneIndex && gameManager.startGame == true)
                ProcessInputs();
        }
        else
            Debug.Log("INPUT LOCK");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!pairedLaser)
            {
                pairedLaser = other.GetComponentInChildren<Laser>();
            }
            if (!storedLaser)
            {
                storedLaser = other.GetComponentInChildren<Laser>();
            }
        }
    }
    #region Inputs
    void ProcessInputs()
    {
        //Once game is over, don't allow movement.
		if ((SceneManager.GetActiveScene().buildIndex == gameManager.LobbySceneIndex && this.GetComponent<CannonCustomization>().hasJoined) ||
			(SceneManager.GetActiveScene().buildIndex == gameManager.mainGameSceneIndex && gameManager.gameOver == false))
        {
            GetRotationInput();
            RestrictAngle();
        }
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
        if (rewiredPlayer.GetButtonDown("Fire"))
        {
            Debug.Log("fire.");
            //StartCoroutine(TempDisableCollider());
            Rigidbody2D playerRigidbody = storedLaser.GetComponentInChildren<Rigidbody2D>();
            //playerRigidbody.isKinematic = false;
            playerRigidbody.bodyType = RigidbodyType2D.Dynamic; 
            playerRigidbody.AddForce(maxBlastForce * this.transform.up);
            storedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			storedLaser.transform.GetComponent<TrailRenderer> ().enabled = true;
            storedLaser = null;
            inFlight = true;
            Debug.Log(storedLaser);
            this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
            this.GetComponent<AudioSource>().Play();
        }
    }
    IEnumerator TempDisableCollider()
    {
        this.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<Collider2D>().enabled = true;
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
    public void ChangeRotationSpeed(float increment)
    {
        baseRotationSpeed += increment;
        baseRotationSpeed = Mathf.Clamp(baseRotationSpeed, minRotationSpeed, maxRotationSpeed);
    }
    public void ChangeColor()
    {
        // New color (Random for now, pull from an array later)
        Color newColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        // Change colors
        SpriteRenderer[] spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in spriteRenderers) { sprite.color = newColor; }
        storedLaser.ChangeColor(newColor);
    }
    public void ModifyRotationSpeed(float newSpeed)
    {
        baseRotationSpeed = newSpeed;
    }
    public void SetStoredLaser(Laser laser)
    { 

        storedLaser = laser;
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