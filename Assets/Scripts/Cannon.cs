using Rewired;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] int playerId;
	[SerializeField] float rotationSpeed = 2;
	[SerializeField] float maxBlastForce = 2200;
	[SerializeField] float maxAngleOffset = 70;

	Player rewiredPlayer;
	Laser storedLaser;
	float currentAngle;
	float angleOffset;
	float baseAngle;
	float minAngle;
	float maxAngle;
	int rotationModifier = 1;

	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer (playerId);
		if (maxAngleOffset < 0) { maxAngleOffset *= -1; }
		SetNewBaseAngle ();
	}

	void Update () {

		if (storedLaser) {

			ProcessInputs ();
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (!storedLaser && other.CompareTag ("Player")) {

			storedLaser = other.GetComponentInChildren<Laser> ();
		}
	}

	void ProcessInputs () {

		// Change direction of rotation if upside down
		if (this.transform.up == Vector3.up) {

			rotationModifier = 1;
		}
		else if (this.transform.up == Vector3.down) {

			rotationModifier = -1;
		}

		// Get controller joystick input
		if (rewiredPlayer.GetAxis ("Horizontal") < 0) {
			//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
			this.transform.Rotate(rotationSpeed * rotationModifier * Vector3.forward);
		}
		else if(rewiredPlayer.GetAxis ("Horizontal") > 0) {
			//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
			this.transform.Rotate(-rotationSpeed * rotationModifier * Vector3.forward);
		}

		// Restrict angle
		currentAngle = this.transform.rotation.eulerAngles.z;
		if(currentAngle < 0) { currentAngle += 360; }

		angleOffset = Mathf.Abs(currentAngle - baseAngle);

		//Debug.Log ("zAngle: " + zAngle);

		if (currentAngle >= maxAngle && currentAngle <= maxAngle + 5) {

			this.transform.rotation = Quaternion.Euler (0, 0, maxAngle);
		}
		else if (currentAngle <= minAngle && currentAngle >= minAngle - 5) {

			this.transform.rotation = Quaternion.Euler (0, 0, minAngle);
		}

		if (rewiredPlayer.GetButtonDown ("Fire")) {
			//Debug.Log ("Fire!");
			StartCoroutine (TempDisableCollider ());
			ShootOutPlayer(storedLaser.GetComponentInChildren<Rigidbody2D>());
			this.GetComponent<AudioSource> ().pitch = Random.Range (0.5f, 1.5f);
			this.GetComponent<AudioSource> ().Play ();
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
