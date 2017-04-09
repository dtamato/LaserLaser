using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpikePlatform : MonoBehaviour {

	[SerializeField] float minY;
	[SerializeField] float maxY;

	bool movingUp = true;


	void Update () {

		if(movingUp) {

			this.transform.Translate(Vector3.up * 0.1f);

			if(this.transform.localPosition.y >= maxY) {

				movingUp = !movingUp;
			}
		}
		else {

			this.transform.Translate(Vector3.down * 0.1f);

			if(this.transform.localPosition.y <= minY) {

				movingUp = !movingUp;
			}
		}
	}
}
