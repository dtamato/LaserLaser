using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] float rotationSpeed;
	[SerializeField] float maxBlastForce;
	[SerializeField] Color idleColor;
	[SerializeField] Color activeColor;

	SpriteRenderer spriteRenderer;
	Player storedPlayer;
	Quaternion originalRotation;

	void Awake () {

		spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.color = idleColor;
		originalRotation = this.transform.rotation;
	}

	void Update () {

		if (storedPlayer) {

			if (Input.GetAxis ("Horizontal") < 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(rotationSpeed * Vector3.forward);
			}
			else if(Input.GetAxis ("Horizontal") > 0) {
				//Debug.Log ("Horizontal: " + Input.GetAxis ("Horizontal"));
				this.transform.Rotate(-rotationSpeed * Vector3.forward);
			}


			if (Input.GetButtonDown ("Jump")) {
				//Debug.Log ("Fire!");
				StartCoroutine (TempDisableCollider ());
				ShootOutPlayer(storedPlayer.GetComponentInChildren<Rigidbody2D>());
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (!storedPlayer && other.CompareTag ("Player")) {

			storedPlayer = other.GetComponentInChildren<Player> ();
			spriteRenderer.color = activeColor;
		}
	}

	IEnumerator TempDisableCollider () {

		this.GetComponent<Collider2D> ().enabled = false;

		yield return new WaitForSeconds (0.5f);

		this.GetComponent<Collider2D> ().enabled = true;

		ResetCannon();
	}

	void ShootOutPlayer (Rigidbody2D playerRigidbody) {

		playerRigidbody.isKinematic = false;
		playerRigidbody.AddForce (maxBlastForce * this.transform.up);
		storedPlayer = null;
	}

	void ResetCannon () {

		spriteRenderer.color = idleColor;
		this.transform.rotation = originalRotation;
	}
}
