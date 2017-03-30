using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Bouncer : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float appearTime = 1;
	[SerializeField] float killTime = 5;

	[Header("Audio")]
	[SerializeField] AudioClip bouncerHitAudioClip;

	Animator animator;
	float appearTimer;

	void Start () {

		animator = this.GetComponentInChildren<Animator> ();
		appearTimer = 0;
	}

	void Update () {

		if(appearTimer < appearTime) {

			appearTimer += Time.deltaTime;
		}
		else if(appearTimer >= appearTime && this.GetComponent<SpriteRenderer>().enabled == false) {

			this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<Collider2D>().isTrigger = false;
			this.GetComponent<Animator>().enabled = true;
			StartCoroutine (StartShrinkAnimation ());
		}
	}

	void OnCollisionEnter2D(Collision2D other) {

        if (other.transform.CompareTag("Player"))
        {
            animator.SetTrigger("Bounce");
			this.GetComponent<AudioSource>().clip = bouncerHitAudioClip;
			this.GetComponent<AudioSource>().Play();
            this.GetComponent<SpriteRenderer>().color = other.transform.GetComponent<SpriteRenderer>().color;
            StartCoroutine(RestoreColor());
            Camera.main.GetComponent<CameraEffects>().ShakeCamera();

            if (other.transform.GetComponent<MenuLaser>())
            {
                other.transform.GetComponent<MenuLaser>().StartCoroutine("Bump", 0.3f);
            }
            else if (other.transform.GetComponent<Laser>())
            {         
                other.transform.GetComponent<Laser>().StartCoroutine("Bump", 0.3f);
            }
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
