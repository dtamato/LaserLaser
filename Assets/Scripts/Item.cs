using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Destroy(this.gameObject, 10);
	}

	void Update () {

		Color currentColor = this.GetComponentInChildren<SpriteRenderer>().color;
		this.GetComponentInChildren<SpriteRenderer>().color = currentColor - new Color(0, 0, 0, 0.1f * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.transform.name.Contains("Laser")) {

			this.GetComponent<Collider2D> ().enabled = false;
			this.GetComponent<SpriteRenderer> ().enabled = false;
			this.GetComponent<AudioSource> ().Play ();
			AudioClip clip = this.GetComponent<AudioSource> ().clip;
			Destroy(this.gameObject, clip.length);
		}
	}
}
