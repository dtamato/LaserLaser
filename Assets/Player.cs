using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

	[SerializeField] GameObject cannon;
	[SerializeField] Text scoreText;

	Rigidbody2D rb2d;
	int score = 0;

	void Awake () {

		rb2d = this.GetComponentInChildren<Rigidbody2D> ();
	}

	void OnCollisionEnter2D(Collision2D other) {

		if(other.transform.CompareTag("Boundary")) {

			rb2d.isKinematic = true;
			cannon.transform.position = other.contacts[0].point;
			cannon.transform.rotation = other.transform.rotation;
			this.transform.position = cannon.transform.position + cannon.transform.up;
			this.transform.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		//Destroy(this.gameObject);
		if(other.CompareTag("Item")) {

			score++;
			scoreText.text = "1: " + score.ToString("00");
			Destroy(other.gameObject);
		}
	}
}
