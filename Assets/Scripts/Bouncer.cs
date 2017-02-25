using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Bouncer : MonoBehaviour {

	[SerializeField] float killTime = 5;

	Animator animator;


	void Start () {

		animator = this.GetComponentInChildren<Animator> ();
		StartCoroutine (StartShrinkAnimation ());
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.transform.CompareTag ("Player")) {

			animator.SetTrigger ("Bounce");
			this.GetComponent<AudioSource> ().Play ();
			this.GetComponent<SpriteRenderer> ().color = other.transform.GetComponent<SpriteRenderer> ().color;
			StartCoroutine (RestoreColor ());
			Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();
		}
	}

	IEnumerator RestoreColor () {

		yield return new WaitForSeconds (0.15f);
		this.GetComponent<SpriteRenderer> ().color = Color.white;
	}

	IEnumerator StartShrinkAnimation () {

		yield return new WaitForSeconds (0.9f * killTime);
		animator.SetTrigger ("Shrink");
	}

	// Called from animator controller
	public void DestroyObject () {

		Destroy (this.gameObject);
	}
}
