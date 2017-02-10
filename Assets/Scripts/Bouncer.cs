using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Bouncer : MonoBehaviour {

	Animator animator;


	void Start () {

		animator = this.GetComponentInChildren<Animator> ();
		this.transform.localScale = Vector3.one;
		Destroy (this.gameObject, 5);
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.transform.CompareTag ("Player")) {

			animator.SetTrigger ("Bounce");
			this.GetComponent<SpriteRenderer> ().color = other.transform.GetComponent<SpriteRenderer> ().color;
			StartCoroutine (RestoreColor ());
		}
	}

	IEnumerator RestoreColor () {

		yield return new WaitForSeconds (0.15f);
		this.GetComponent<SpriteRenderer> ().color = Color.white;
	}
}
