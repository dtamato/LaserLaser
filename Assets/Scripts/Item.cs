using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Item : MonoBehaviour {

	[SerializeField] GameObject diamondCollectedPrefab;

	SpriteRenderer spriteRenderer;
	Light diamondLight;
	float flickerLength;
	float flickerTimer;
	int shrinkingDirection;

	void Start () {

		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		diamondLight = this.GetComponentInChildren<Light> ();
		flickerLength = 0.1f;
		flickerTimer = flickerLength;
		shrinkingDirection = -1;
		Destroy(this.gameObject, 10);
	}

	void Update () {

		FadeColors ();
		FlickerLight ();
	}

	void FadeColors () {

		Color newColor = spriteRenderer.color - new Color(0, 0, 0, 0.1f * Time.deltaTime);
		spriteRenderer.color = newColor;
		diamondLight.color = newColor;
		diamondLight.range -= 0.25f * Time.deltaTime;
	}

	void FlickerLight () {

		flickerTimer -= Time.deltaTime;
		diamondLight.intensity += 3f * shrinkingDirection * Time.deltaTime;

		if (flickerTimer <= 0) {

			shrinkingDirection *= -1;
			flickerTimer = flickerLength;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.transform.name.Contains("Laser") || other.transform.name.Contains("Scatter Object")) {

			this.GetComponent<Collider2D> ().enabled = false;
			spriteRenderer.enabled = false;
			diamondLight.enabled = false;
			this.GetComponent<AudioSource> ().Play ();
			AudioClip clip = this.GetComponent<AudioSource> ().clip;

			GameObject diamondCollected = Instantiate (diamondCollectedPrefab, this.transform.position, Quaternion.identity) as GameObject;
			diamondCollected.GetComponent<CircleParticle> ().ChangeColor (other.GetComponentInChildren<SpriteRenderer> ().color);

			Destroy(this.gameObject, clip.length);
		}
	}
}
