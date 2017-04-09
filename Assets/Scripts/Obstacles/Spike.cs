using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Spike : MonoBehaviour {

	Vector3 startPosition;
	Vector3 endPosition;

	void Awake () {

		startPosition = new Vector3(0f, this.transform.localPosition.y, this.transform.localPosition.z);
		endPosition = new Vector3(1f, this.transform.localPosition.y, this.transform.localPosition.z);
	}

	void Update () {

		this.transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.PingPong(0.5f * Time.time + (this.transform.localPosition.y * 0.1f * Mathf.Sign(this.transform.position.x)), 1.0f));
	}
}
