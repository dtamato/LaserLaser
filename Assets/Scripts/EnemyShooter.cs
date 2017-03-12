using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class EnemyShooter : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float moveSpeed = 1;
	[SerializeField] float minGunAngle = 0;
	[SerializeField] float maxGunAngle = 180;
	[SerializeField] float gunRotationSpeed = 1;
	[SerializeField] float shotCooldown = 1;

	[Header("References")]
	[SerializeField] GameObject enemyBulletPrefab;
	[SerializeField] Transform[] patrolNodes;
	[SerializeField] GameObject enemyGunPivot;
	[SerializeField] Transform shootTransform;

	Transform currentNode;
	Transform nextNode;
	float journeyLength;
	float startTime;
	int nodeIndex = 0;
	bool rotatingCCW = true;
	float shotTimer = 0;

	GameObject player = null;

	void Awake () {

		startTime = Time.time;

		if(patrolNodes.Length > 1) { 

			nodeIndex++;
			currentNode = patrolNodes[0];
			nextNode = patrolNodes[1];
			journeyLength = Vector3.Distance(currentNode.position, nextNode.position);
		}
	}

	void Update () {

		if(player == null) {
			
			MoveEnemy();
			MoveGun();
		}
		else {

			ShootEnemy();
		}

		UpdateTimers();
	}

	void MoveEnemy () {

		if(patrolNodes.Length > 1) {

			float distCovered = (Time.time - startTime) * moveSpeed;
			float fracJourney = distCovered / journeyLength;

			this.transform.position = Vector3.Lerp(currentNode.position, nextNode.position, fracJourney);

			if(Vector3.Distance(this.transform.position, nextNode.position) < 0.1f) {
				
				nodeIndex++;
				nodeIndex %= patrolNodes.Length;

				currentNode = nextNode;
				nextNode = patrolNodes[nodeIndex];

				startTime = Time.time;
				journeyLength = Vector3.Distance(currentNode.position, nextNode.position);
			}
		}
	}

	void MoveGun () {

		if(rotatingCCW) {

			enemyGunPivot.transform.Rotate(gunRotationSpeed * Vector3.forward);
		}
		else {

			enemyGunPivot.transform.Rotate(-gunRotationSpeed * Vector3.forward);
		}

		// Switch direction
		float zAngle = enemyGunPivot.transform.rotation.eulerAngles.z;
		if(zAngle > maxGunAngle - 5 && rotatingCCW) { rotatingCCW = !rotatingCCW; }
		if(zAngle > maxGunAngle - 5 && !rotatingCCW) { rotatingCCW = !rotatingCCW; }
	}

	void UpdateTimers () {

		shotTimer -= Time.deltaTime;
	}

	void ShootEnemy () {

		Vector3 vectorToPlayer = player.transform.position - this.transform.position + player.transform.up;
		float angleFromPlayer = Vector3.Angle(enemyGunPivot.transform.right, vectorToPlayer);
		//Debug.Log("angle from player: " + angleFromPlayer);

		if(angleFromPlayer < 5 && shotTimer <= 0) {

			//Debug.Log("Shooting player");
			GameObject newBullet = Instantiate(enemyBulletPrefab, shootTransform.position, enemyGunPivot.transform.rotation) as GameObject;
			shotTimer = shotCooldown;
		}
		else {

			enemyGunPivot.transform.Rotate(3 * Vector3.forward);
		}
	}

	void OnCollisionEnter2D (Collision2D other) {

		if(other.transform.CompareTag("Player")) {

			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {

		if(other.CompareTag("Cannon") || other.CompareTag("Laser")) {
			
			player = other.gameObject;
		}
	}

	void OnTriggerExit2D (Collider2D other) {

		if(other.CompareTag("Cannon") || other.CompareTag("Laser")) {

			player = null;
		}
	}
}
