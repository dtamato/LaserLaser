using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour {

	[SerializeField] GameObject itemPrefab;
	[SerializeField] float spawnCooldown = 1.0f;

    private GameManager gameManager;
    private bool initial = true;
    private bool cooldown = false;

	void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	void Update ()
    {
        //Start spawning items once the game countdown has finished.
	    if (gameManager.startGame && initial) {
            initial = false;
            StartCoroutine("SpawnItem");
        }
        //Continue spawning items each time the cooldown is met.
        if (gameManager.startGame && cooldown) {
            cooldown = false;
            StartCoroutine("SpawnItem");
        }
        //When there's only 30 seconds left, speed up item spawning.
        if (gameManager.timer < 15.0f)
            spawnCooldown = 0.5f;
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
}
