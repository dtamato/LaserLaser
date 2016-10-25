using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Cannon : MonoBehaviour {

	[SerializeField] float rotationSpeed;
	[SerializeField] float maxBlastForce;

	Player storedPlayer;

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
				storedPlayer.GetComponent<Rigidbody2D>().isKinematic = false;
				storedPlayer.GetComponent<Rigidbody2D>().AddForce (maxBlastForce * this.transform.up);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Player")) {

			storedPlayer = other.GetComponentInChildren<Player> ();
			this.GetComponentInChildren<SpriteRenderer> ().color = Color.green;
		}
	}

	IEnumerator TempDisableCollider () {

		this.GetComponent<Collider2D> ().enabled = false;

		yield return new WaitForSeconds (0.5f);

		this.GetComponent<Collider2D> ().enabled = true;
	}
}
