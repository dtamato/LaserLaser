using UnityEngine;
using System.Collections;

public class PowerupSpawner : MonoBehaviour 
{
	[SerializeField] private GameObject[] powerUpPrefabs;
	[SerializeField] private float spawner;
	private BaseGM gameManager;

	void Start () 
	{
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<BaseGM> ();
		StartCoroutine (RecursiveSpawner ());
	}

	IEnumerator RecursiveSpawner () 
	{
		if (gameManager.startGame) {
			Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
			float newX = Random.Range (spawnerBounds.min.x, spawnerBounds.max.x);
			float newY = Random.Range (spawnerBounds.min.y, spawnerBounds.max.y);
			Vector3 powerUpLocation = new Vector3 (newX, newY, 0);
			GameObject powerUp = Instantiate (powerUpPrefabs [Random.Range (0, powerUpPrefabs.Length)], powerUpLocation, Quaternion.identity) as GameObject;
			powerUp.transform.SetParent (this.transform);
		}

		yield return new WaitForSeconds (Random.Range(spawner * 0.5f, spawner * 1.5f));

		StartCoroutine (RecursiveSpawner ());
	}
}
