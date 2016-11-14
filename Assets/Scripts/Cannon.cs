using Rewired;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] int playerId;
	[SerializeField] float rotationSpeed;
	[SerializeField] float maxBlastForce;

	Player rewiredPlayer;
	Laser storedLaser;
	float pivotAngle;
	int rotationModifier = 1;

	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer (playerId);
	}

	void Update () {

		if (storedLaser) {

			// Change direction of rotation
			if (this.transform.up == Vector3.up) {

				rotationModifier = 1;
			}
			else if (this.transform.up == Vector3.down) {

				rotationModifier = -1;
			}

			if (rewiredPlayer.GetAxis ("Horizontal") < 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(rotationSpeed * rotationModifier * Vector3.forward);
			}
			else if(rewiredPlayer.GetAxis ("Horizontal") > 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(-rotationSpeed * rotationModifier * Vector3.forward);
			}

			// Restrict angle
//			float zAngle = this.transform.rotation.eulerAngles.z;
//			float offsetAngle = pivotAngle - zAngle;
//			//Debug.Log ("zAngle: " + zAngle);
//			if ((zAngle >= -45 && zAngle <= -40) ||zAngle >= 315 && zAngle <= 320) {
//
//				this.transform.rotation = Quaternion.Euler (0, 0, -39);
//			}
//			else if (zAngle >= 40 && zAngle <= 45) {
//
//				this.transform.rotation = Quaternion.Euler (0, 0, 39);
//			}


			if (rewiredPlayer.GetButtonDown ("Fire")) {
				//Debug.Log ("Fire!");
				StartCoroutine (TempDisableCollider ());
				ShootOutPlayer(storedLaser.GetComponentInChildren<Rigidbody2D>());
				this.GetComponent<AudioSource> ().pitch = Random.Range (0.5f, 1.5f);
				this.GetComponent<AudioSource> ().Play ();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (!storedLaser && other.CompareTag ("Player")) {

			storedLaser = other.GetComponentInChildren<Laser> ();
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

//	// Called from Laser.cs
//	public void SetPivotAngle (float newPivotAngle) {
//
//		pivotAngle = newPivotAngle;
//	}

	public int GetPlayerID () {

		return playerId;
	}
}
