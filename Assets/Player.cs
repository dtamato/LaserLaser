using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

	Rigidbody2D rb2d;

	void Awake () {

		rb2d = this.GetComponentInChildren<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Cannon")) {

			rb2d.isKinematic = true;
			this.transform.position = other.transform.position;
		}
	}
}
