using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class ObstacleSpawner : MonoBehaviour {

	[SerializeField] GameObject[] obstaclePrefabs;
	[SerializeField] float spawnRate;

	BaseGM gameManager;

	void Start () {

		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<BaseGM> ();
		StartCoroutine (RecursiveSpawner ());
	}

	IEnumerator RecursiveSpawner () {

		if (gameManager.startGame) {

			Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
			float newX = Random.Range (spawnerBounds.min.x, spawnerBounds.max.x);
			float newY = Random.Range (spawnerBounds.min.y, spawnerBounds.max.y);
			Vector3 randomLocation = new Vector3 (newX, newY, 0);
			GameObject newObstacle = Instantiate (obstaclePrefabs [Random.Range (0, obstaclePrefabs.Length)], randomLocation, Quaternion.identity) as GameObject;
			newObstacle.transform.SetParent (this.transform);
		}

		yield return new WaitForSeconds (Random.Range(spawnRate * 0.5f, spawnRate * 1.5f));

		StartCoroutine (RecursiveSpawner ());
	}
}
