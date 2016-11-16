﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Laser : MonoBehaviour {

	[SerializeField] GameObject cannon;
	[SerializeField] Text scoreText;

	Rigidbody2D rb2d;
	int score = 0;

	void Awake () {

		rb2d = this.GetComponentInChildren<Rigidbody2D> ();
		this.GetComponentInChildren<TrailRenderer> ().sortingLayerName = this.GetComponent<SpriteRenderer> ().sortingLayerName;
		this.GetComponentInChildren<TrailRenderer> ().sortingOrder = this.GetComponent<SpriteRenderer> ().sortingOrder - 1;
	}

	void OnCollisionEnter2D(Collision2D other) {

		if(other.transform.CompareTag("Boundary")) {

			rb2d.isKinematic = true;
			cannon.transform.position = other.contacts[0].point;
			cannon.transform.rotation = other.transform.rotation;
			this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
			this.transform.GetComponent<SpriteRenderer>().enabled = false;
			cannon.GetComponentInChildren<Cannon> ().SetNewBaseAngle ();
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		//Destroy(this.gameObject);
		if(other.CompareTag("Item")) {

			score++;
			scoreText.text = "P" + (cannon.GetComponent<Cannon> ().GetPlayerID () + 1) + "- " + score.ToString("00");
		}
	}
}
