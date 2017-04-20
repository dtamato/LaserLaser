using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEnemy : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			Camera.main.GetComponent<SinglePlayerCamera>().ShowEnemy();
			Destroy(this.gameObject);
		}
	}
}
