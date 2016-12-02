using Rewired;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] int playerId;
	[SerializeField] float rotationSpeed = 2;
	[SerializeField] float maxBlastForce = 2200;
	[SerializeField] float maxAngleOffset = 70;

    // Variables handling player rotation.
	float currentRotation;
	float accelerationModifier = 0.01f;
	float currentAcceleration = 0;
	int rotationModifier = 1;
	float currentAngle;
	float angleOffset;
	float baseAngle;
	float minAngle;
	float maxAngle;

	// UI variables
    GameObject joinUI = null;
    Text scoreUI = null;
    Text comboUI = null;

	// External References.
	Player rewiredPlayer;
	Laser storedLaser;

	void Awake ()
    {
        rewiredPlayer = ReInput.players.GetPlayer (playerId);
        //Debug.Log(rewiredPlayer);
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
        SetNewBaseAngle();
    }

    void UpdateColor()
    {
        transform.Find("Cannon Sprite" + playerId).GetComponent<SpriteRenderer>().color =
            gameObject.GetComponentInChildren<Laser>().GetComponent<SpriteRenderer>().color; //updates all parts of the cannon
    }

	void Update ()
    {
		if (storedLaser) {
				
            ProcessInputs ();
            UpdateColor();
        }
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!storedLaser && other.CompareTag ("Player"))
			storedLaser = other.GetComponentInChildren<Laser> ();
	}

	void ProcessInputs ()
	{
	    
        // Get controller joystick input
		if (rewiredPlayer.GetAxis ("Horizontal") < 0) {
			
			//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
			rotationSpeed += accelerationModifier;
			this.transform.Rotate (rotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxis ("Horizontal") > 0) {
			
			//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
			this.transform.Rotate (-rotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxis ("Horizontal") == 0) {

			currentAcceleration = 0;
		}

		RestrictAngle ();

        if (rewiredPlayer.GetButtonDown("Fire"))
        {
            StartCoroutine(TempDisableCollider());
            ShootOutPlayer(storedLaser.GetComponentInChildren<Rigidbody2D>());
            this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
            this.GetComponent<AudioSource>().Play();
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

		// Change rotation modifier if upside down
		rotationModifier = (this.transform.up == Vector3.down) ? -1 : 1;
	}

	public int GetPlayerID () {

		return playerId;
	}
}
