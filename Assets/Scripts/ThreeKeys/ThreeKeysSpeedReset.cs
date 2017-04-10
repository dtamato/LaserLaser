using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeKeysSpeedReset : MonoBehaviour {

	[SerializeField] float moveSpeed = 5;

	Vector3 startPosition;
	float initialMoveSpeed;

	void Awake () {

		startPosition = this.transform.position;
		initialMoveSpeed = moveSpeed;
	}

	void Update () {

		this.transform.Translate(moveSpeed * Vector3.left * Time.deltaTime);
		moveSpeed += 10 * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			other.transform.position = startPosition;
			this.transform.position = startPosition;
			moveSpeed = initialMoveSpeed;
			this.gameObject.SetActive(false);
		}
	}
}
