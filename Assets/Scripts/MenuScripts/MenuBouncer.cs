using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuBouncer : MonoBehaviour {

	[SerializeField] Image canvasTimer;
	[SerializeField] Animator menuBouncerAnimator;

	void Awake () {

		menuBouncerAnimator = this.GetComponentInChildren<Animator>();
	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.transform.CompareTag("Player"))
		{
			//menuBouncerAnimator.SetTrigger("Bounce");
			this.GetComponent<AudioSource>().Play();
			canvasTimer.color = other.transform.GetComponent<SpriteRenderer>().color;
			StartCoroutine(RestoreColor());
			Camera.main.GetComponent<CameraEffects>().ShakeCamera();

 			if (other.transform.GetComponent<Laser>())
			{         
				other.transform.GetComponent<Laser>().StartCoroutine("Bump", 0.3f);
			}
		}
	}

	IEnumerator RestoreColor () {

		yield return new WaitForSeconds (0.15f);
		canvasTimer.color = Color.white;
	}
}
