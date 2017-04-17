using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class SinglePlayerLaser : MonoBehaviour {

	[SerializeField] GameObject cannon;
	Rigidbody2D rb2d;

	void Awake()
	{
		rb2d = this.GetComponentInChildren<Rigidbody2D>();
		this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
		this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.CompareTag("Boundary"))
		{
			rb2d.bodyType = RigidbodyType2D.Static;
			cannon.transform.position = other.contacts[0].point;
			cannon.transform.rotation = other.transform.rotation;
			this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
			this.transform.GetComponent<SpriteRenderer>().enabled = false;
			this.transform.GetComponent<TrailRenderer> ().enabled = false;
			cannon.GetComponentInChildren<SinglePlayerCannon>().SetNewBaseAngle();
			cannon.GetComponentInChildren<SinglePlayerCannon> ().SetStoredLaser (this);
		}
		else if(other.transform.name == "Enemy Shooter") {

			this.GetComponent<AudioSource>().Play();
		}
	}

	public GameObject GetCannon () {

		return cannon;
	}
}
