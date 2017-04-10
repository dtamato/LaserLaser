using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ThreeKeysButton : MonoBehaviour {

	[SerializeField] Area areaType;
	[SerializeField] ThreeKeysController gameController;

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			this.GetComponentInChildren<SpriteRenderer>().color = Color.green;
			this.GetComponentInChildren<Light>().color = Color.green;
			gameController.FinishArea(areaType);
		}
	}
}
