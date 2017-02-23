using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ChangeCameraClamps : MonoBehaviour {

	[SerializeField] float newMinX = 0;
	[SerializeField] float newMaxX = 0;
	[SerializeField] float newMinY = 0;
	[SerializeField] float newMaxY = 0;


	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Player")) {

			Camera.main.GetComponent<CameraSmoothFollow> ().ChangeClamps (newMinX, newMaxX, newMinY, newMaxY);
		}
	}
}
