using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour {

	[SerializeField] GameObject itemPrefab;
	[SerializeField] float spawnCooldown = 1.0f;

    private GameManager gameManager;
    private bool initial = true;
    private bool cooldown = false;
	private bool dontRunThisAlotOfTimes = false;
	private int playerCount = 0;

	void Start ()
    {
		if (GameObject.FindGameObjectWithTag ("GameManager")) {
		
			gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();

			//Check how many players are in the game.
			for (int i = 0; i < 4; i++) {
				if (gameManager.playerState [i] = true)
					playerCount++;
			}
		}

		//Scale the spawn cooldown based on number of players.
		spawnCooldown = 1.0f - (0.05f * playerCount);

		if(SceneManager.GetActiveScene().name == "Powerup Test Arena") {

			StartCoroutine ("RecursiveSpawner");
		}
	}
	
	void Update ()
    {
		if (gameManager) {
			
			//Start spawning items once the game countdown has finished.
			if (gameManager.startGame && initial) {
				initial = false;
				StartCoroutine ("SpawnItem");
			}
			//Continue spawning items each time the cooldown is met.
			if (gameManager.startGame && cooldown) {
				cooldown = false;
				StartCoroutine ("SpawnItem");
			}
			//When there's only 30 seconds left, speed up item spawning.
			if (!dontRunThisAlotOfTimes && gameManager.timer < 30.0f) {
				spawnCooldown -= 0.2f;
				dontRunThisAlotOfTimes = true;
			}
		}
    }

	IEnumerator SpawnItem ()
    {
		Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D>().bounds;
		float newX = Random.Range(spawnerBounds.min.x, spawnerBounds.max.x);
		float newY = Random.Range(spawnerBounds.min.y, spawnerBounds.max.y);
		Vector3 itemLocation = new Vector3(newX, newY, 0);

		GameObject newItem = Instantiate(itemPrefab, itemLocation, Quaternion.identity) as GameObject;
		newItem.transform.SetParent(this.transform);

		yield return new WaitForSeconds(spawnCooldown);
        cooldown = true;
	}

	IEnumerator RecursiveSpawner () {

		StartCoroutine ("SpawnItem");

		yield return new WaitForSeconds (spawnCooldown);

		StartCoroutine ("RecursiveSpawner");
	}
}
