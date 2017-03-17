using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MenuDiamond : MonoBehaviour {

	[SerializeField] GameObject diamondCollectedPrefab;

	SpriteRenderer spriteRenderer;


	void Start () {

		spriteRenderer = this.GetComponent<SpriteRenderer> ();
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.transform.name.Contains("Laser")) {

			this.GetComponent<Collider2D> ().enabled = false;
			spriteRenderer.enabled = false;
			this.GetComponent<AudioSource> ().Play ();
			AudioClip clip = this.GetComponent<AudioSource> ().clip;

			GameObject diamondCollected = Instantiate (diamondCollectedPrefab, this.transform.position, Quaternion.identity) as GameObject;
			diamondCollected.GetComponent<CircleParticle> ().ChangeColor (other.GetComponentInChildren<SpriteRenderer> ().color);

			Destroy(this.gameObject, clip.length);
		}
	}
}
