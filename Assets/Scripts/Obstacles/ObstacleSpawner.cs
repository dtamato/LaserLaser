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

		if (gameManager.getState() == BaseGM.GAMESTATE.INGAME) {

			Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
            Collider2D bumperRadius = obstaclePrefabs[0].GetComponent<Collider2D>();
			float newX = Random.Range (spawnerBounds.min.x, spawnerBounds.max.x);
			float newY = Random.Range (spawnerBounds.min.y, spawnerBounds.max.y);
			Vector3 randomLocation = new Vector3 (newX, newY, 0);
            GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], randomLocation, Quaternion.identity) as GameObject;
            
            Collider2D[] diamondsFound = Physics2D.OverlapCircleAll(randomLocation, bumperRadius.bounds.extents.x);
            for (int i = 0; i < diamondsFound.Length; i++)
            {
                Debug.Log("DiamondsFound: " + diamondsFound[i].name);
            }

            //bool diamondFound = (.Length > 0) ? true : false;
            //bool obstacleFound = (Physics2D.OverlapCircleAll(randomLocation, bumperRadius.bounds.extents.x, LayerMask.NameToLayer("Obstacle")).Length > 0) ? true : false;
            //GameObject newObstacle = Instantiate (obstaclePrefabs [Random.Range (0, obstaclePrefabs.Length)], randomLocation, Quaternion.identity) as GameObject;
            //Debug.Log("diamondFound: " + diamondFound);
            //Debug.Log("obstacleFound: " + obstacleFound);
            //Debug.Break();
            //if (diamondFound || obstacleFound)
            //{
            //    StartCoroutine(RecursiveSpawner());
            //}
            //else
            //{
            //    GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], randomLocation, Quaternion.identity) as GameObject;
            //}
            //newObstacle.transform.SetParent (this.transform);
        }

		yield return new WaitForSeconds (Random.Range(spawnRate * 0.5f, spawnRate * 1.5f));

		StartCoroutine (RecursiveSpawner ());
	}
}
