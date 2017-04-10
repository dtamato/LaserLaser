using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ThreeKeysCameraSwitcher : MonoBehaviour {

	[SerializeField] float minX;
	[SerializeField] float maxX;
	[SerializeField] float minY;
	[SerializeField] float maxY;


	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			Camera.main.GetComponent<CameraSmoothFollow>().ChangeClamps(minX, maxX, minY, maxY);
		}
	}
}
