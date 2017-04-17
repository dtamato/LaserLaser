using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ControlBouncer : MonoBehaviour {

	SpriteRenderer spriteRenderer;
	Animator animator;


	void Awake () {

		spriteRenderer = this.GetComponent<SpriteRenderer>();
		animator = this.GetComponentInChildren<Animator> ();
	}

	void Update () {

		if(spriteRenderer.color != Color.white) {

			spriteRenderer.color += 0.0005f * Color.white;

			if(spriteRenderer.color.r > 0.75f && spriteRenderer.color.g > 0.75f && spriteRenderer.color.b > 0.75f) { 

				spriteRenderer.color = Color.white;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.transform.CompareTag ("Player")) {

			animator.SetTrigger ("Bounce");
			spriteRenderer.color = other.transform.GetComponent<SpriteRenderer> ().color;
		}
	}
}
