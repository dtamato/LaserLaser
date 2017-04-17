using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LightTarget : MonoBehaviour {

	[SerializeField] float minY = -5;
	[SerializeField] float maxY = 5;
	[SerializeField] float moveSpeed = 1;
	[SerializeField] int moveDirection = 1;


	void Awake () {

		moveDirection = (Random.value < 0.5f) ? 1 : -1;
	}

	void Update () {

		if (this.transform.position.y > minY && this.transform.position.y < maxY) {

			this.transform.Translate (moveSpeed * moveDirection * Vector3.up * Time.deltaTime);
		}
		else {

			this.transform.position = new Vector3 (this.transform.position.x, moveDirection * Mathf.Abs (0.95f * maxY), this.transform.position.z);
			moveDirection *= -1;
			moveSpeed = Random.Range (1, 2f);
		}
	}
}
