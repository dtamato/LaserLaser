using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyBullet : MonoBehaviour {

	[SerializeField] float bulletSpeed = 1;

	void Update () {

		this.GetComponent<Rigidbody2D>().MovePosition(this.transform.position + bulletSpeed * this.transform.right);
	}

	void OnCollisionEnter2D(Collision2D other) {

		if(other.transform.CompareTag("Player")) {

			GameObject cannon = other.transform.GetComponent<SinglePlayerLaser>().GetCannon();
			cannon.GetComponent<SinglePlayerCannon>().ResetPosition();
		}

		Destroy(this.gameObject);
	}
}
