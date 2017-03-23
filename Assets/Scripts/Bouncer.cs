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

        if (other.transform.CompareTag("Player"))
        {

            animator.SetTrigger("Bounce");
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
