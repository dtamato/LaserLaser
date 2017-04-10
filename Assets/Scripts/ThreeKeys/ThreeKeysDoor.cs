using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ThreeKeysDoor : MonoBehaviour {

	[SerializeField] FadeCameraOverlay cameraOverlay;

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			other.transform.position = this.transform.position;
			other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			cameraOverlay.FadeToBlack();
			Application.Quit();
		}
	}
}
