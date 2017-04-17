using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class ObstacleSpawner : MonoBehaviour {

	[SerializeField] GameObject[] obstaclePrefabs;
	[SerializeField] float spawnRate;

	BaseGM gameManager;

    const int diamondLayer = 13;
    const int obstacleLayer = 15;


    void Start () {

		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<BaseGM> ();
		StartCoroutine (RecursiveSpawner ());
	}

	IEnumerator RecursiveSpawner () {

		if (gameManager.getState() == BaseGM.GAMESTATE.INGAME) {

			Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
            int diamondLayerMask = 1 << diamondLayer;
            int obstacleLayerMask = 1 << obstacleLayer;
            int combinedLayerMask = diamondLayerMask | obstacleLayerMask;
            Collider2D[] foundColliders;
            Vector3 randomLocation;

            do {

                float newX = Random.Range(spawnerBounds.min.x, spawnerBounds.max.x);
                float newY = Random.Range(spawnerBounds.min.y, spawnerBounds.max.y);
                randomLocation = new Vector3(newX, newY, 0);

                foundColliders = Physics2D.OverlapCircleAll(randomLocation, 1, combinedLayerMask);

            } while (foundColliders.Length > 0);

            Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], randomLocation, Quaternion.identity);
		}

		yield return new WaitForSeconds (Random.Range(spawnRate * 0.5f, spawnRate * 1.5f));

		StartCoroutine (RecursiveSpawner ());
	}
}
