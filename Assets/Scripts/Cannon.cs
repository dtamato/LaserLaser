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

	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer (playerId);
	}

	void Update () {

		if (storedLaser) {

			if (rewiredPlayer.GetAxis ("Horizontal") < 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(rotationSpeed * Vector3.forward);
			}
			else if(rewiredPlayer.GetAxis ("Horizontal") > 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(-rotationSpeed * Vector3.forward);
			}


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

		ResetCannon();
	}

	void ShootOutPlayer (Rigidbody2D playerRigidbody) {

		playerRigidbody.isKinematic = false;
		playerRigidbody.AddForce (maxBlastForce * this.transform.up);
		storedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
		storedLaser = null;
	}

	void ResetCannon () {


	}

	public int GetPlayerID () {

		return playerId;
	}
}
