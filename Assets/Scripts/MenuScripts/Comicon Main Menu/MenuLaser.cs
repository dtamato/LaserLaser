using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MenuLaser : MonoBehaviour {

	[SerializeField] GameObject menuPlayer;

	// Private variables
	Rigidbody2D rb2d;


	void Awake()
	{
		rb2d = this.GetComponentInChildren<Rigidbody2D>();
		this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
		this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.CompareTag ("Boundary")) {

			rb2d.bodyType = RigidbodyType2D.Static; // HERE
			menuPlayer.transform.position = other.contacts [0].point;
			menuPlayer.transform.rotation = other.transform.rotation;
			menuPlayer.transform.GetChild (0).rotation = other.transform.rotation;
			this.transform.localPosition = new Vector3(0, 2f, 0);
			this.transform.GetComponent<SpriteRenderer> ().enabled = false;
			this.transform.GetComponent<TrailRenderer> ().enabled = false;
			menuPlayer.GetComponentInChildren<MenuCannon> ().SetNewBaseAngle ();
			menuPlayer.GetComponentInChildren<MenuCannon> ().SetInFlight (false);
		}
	}
}
