using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] int playerId;
	[SerializeField] float baseRotationSpeed = 2;
	[SerializeField] float maxBlastForce = 2200;
	[SerializeField] float maxAngleOffset = 70;

    private bool isInLobby = true;
    [SerializeField] private bool hasJoined = false;
    public bool canChange = false;

    public int colorIdx;

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

	Player rewiredPlayer;
	Laser pairedLaser; // Permanent reference to ball
	Laser storedLaser; // Reference used to check if can fire

	void Awake ()
    {
        rewiredPlayer = ReInput.players.GetPlayer (playerId);
		currentRotationSpeed = baseRotationSpeed;
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
        SetNewBaseAngle();
    }

    void Start()
    {
        colorIdx = playerId;
        //if(isInLobby)
            //GameObject.Find("Player" + (playerId + 1) + " Overlay").GetComponent<LobbyManager>().UnjoinColour();
    }

    void Update ()
    {
		if (storedLaser) {
            ProcessInputs ();
        }
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag ("Player")) {

			if (!pairedLaser) {

				pairedLaser = other.GetComponentInChildren<Laser> ();
			}

			if (!storedLaser) {

				storedLaser = other.GetComponentInChildren<Laser> ();
			}
		}
	}

    void PlayerActivationCheck()
    {
        if (hasJoined)
            gameObject.GetComponent<Cannon>().enabled = true; //if the player has joined, allow them to control the cannon, else disable it (because this is now in the Cannon script, need to find a way to disable movement)
        else
            gameObject.GetComponent<Cannon>().enabled = false;
    }


    #region Inputs
    void ProcessInputs ()
    {
        if (!isInLobby)
        {
            GetRotationInput();
            RestrictAngle();
            GetFireInput();
        }
        else
        {
            
            if (!hasJoined)
            {
                LobbyInputs();
            }
            else
            {
                LobbyInputs();
                GetFireInput();
                GetRotationInput();
                RestrictAngle();
            }
        }

    }

    void LobbyInputs()
    {
        if (rewiredPlayer.GetButtonDown("Fire") && !hasJoined)
        {
            GameObject.Find("Player" + (playerId + 1) + " Overlay").GetComponent<LobbyManager>().JoinColour();
            hasJoined = true;
            GameObject.Find("JoinText" + playerId).GetComponent<Text>().enabled = false;


        }
        else if (rewiredPlayer.GetButtonDown("Back") && hasJoined)
        {
            hasJoined = false;
            //gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.grey; //graying out the player to show that it is inactive
            GameObject.Find("Player" + (playerId + 1) + " Overlay").GetComponent<LobbyManager>().UnjoinColour();
        }

        if (rewiredPlayer.GetButtonDown("RButt") && canChange) //can only change if the player is within the field
        {
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = true; //accessing the array of available colours contained within ColorManager
            do
            {
                colorIdx++;
                colorIdx %= 10;
                //Debug.Log(colorIdx);
                
            } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable);
            GameObject.Find("Player" + (playerId + 1) + " Overlay").GetComponent<LobbyManager>().UpdateColour();

        }

        if (rewiredPlayer.GetButtonDown("LButt") && canChange)
        {
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = true;
            do
            {

                colorIdx = (colorIdx - 1) % 10;
                colorIdx = colorIdx < 0 ? colorIdx + 10 : colorIdx; //is check 1 true? if yes, use check 2 (wraps around back to the end of the array when you're decrementing past the first element)
                

                //Debug.Log(colorIdx);
            } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable);
            GameObject.Find("Player" + (playerId + 1) + " Overlay").GetComponent<LobbyManager>().UpdateColour();
        }



        if (rewiredPlayer.GetButtonDown("Setting"))
        {
            Debug.Log("Opening Settings...");
        }
    }

	void GetRotationInput () {

		// Get controller joystick input
		if (rewiredPlayer.GetAxisRaw ("Horizontal") < 0) {

			currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			this.transform.Rotate (currentRotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxisRaw ("Horizontal") > 0) {

			currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			this.transform.Rotate (-currentRotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxisRaw ("Horizontal") == 0) {

			currentRotationSpeed = baseRotationSpeed;
		}
	}

	void RestrictAngle () {

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

	void GetFireInput () {

		if (rewiredPlayer.GetButtonDown ("Fire")) {

			StartCoroutine(TempDisableCollider());

			Rigidbody2D playerRigidbody = storedLaser.GetComponentInChildren<Rigidbody2D> ();
			playerRigidbody.isKinematic = false;
			playerRigidbody.AddForce (maxBlastForce * this.transform.up);

			storedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			storedLaser = null;

			this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
			this.GetComponent<AudioSource>().Play();
		}
	}

	IEnumerator TempDisableCollider () {

		this.GetComponent<Collider2D> ().enabled = false;

		yield return new WaitForSeconds (0.15f);

		this.GetComponent<Collider2D> ().enabled = true;
	}
	#endregion

	#region Setters
	// Called from Laser.cs
	public void SetNewBaseAngle () {

		baseAngle = this.transform.rotation.eulerAngles.z;
		if (baseAngle < 0) { baseAngle %= 360; }

		minAngle = baseAngle - maxAngleOffset;
		if (minAngle < 0) { minAngle += 360; }

		maxAngle = baseAngle + maxAngleOffset;
		if (maxAngle < 0) { maxAngle += 360; }

		// Change rotation modifier if upside down
		rotationModifier = (this.transform.up == Vector3.down) ? -1 : 1;
	}

	public void ChangeRotationSpeed (float increment) {

		baseRotationSpeed += increment;
		baseRotationSpeed = Mathf.Clamp (baseRotationSpeed, minRotationSpeed, maxRotationSpeed);
	}

	public void ChangeColor () {

		// New color (Random for now, pull from an array later)
		Color newColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

		// Change colors
		SpriteRenderer[] spriteRenderers = this.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in spriteRenderers) { sprite.color = newColor; }
		storedLaser.ChangeColor (newColor);
	}

	public void ModifyRotationSpeed(float newSpeed)
	{
		baseRotationSpeed = newSpeed;
	}
	#endregion

	#region Getters
	public int GetPlayerID () {

		return playerId;
	}

	public float GetBaseSpeed()
	{
		return baseRotationSpeed;
	}
	#endregion
}