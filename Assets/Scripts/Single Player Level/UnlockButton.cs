using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UnlockButton : MonoBehaviour {


	void OnCollisionEnter2D (Collision2D other) {

		if(other.transform.CompareTag("Player")) {

			Camera.main.GetComponent<SinglePlayerCamera>().ShowDoor();
			Destroy(this.gameObject);
		}
	}
}
