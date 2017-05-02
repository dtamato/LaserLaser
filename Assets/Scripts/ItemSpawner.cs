using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    
    private BaseGM gameManager;
	private float spawnCooldown;

    const int diamondLayer = 13;
    const int obstacleLayer = 15;

	void Start()
    {
        // Scale the spawn cooldown based on number of players.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        int playerCount = gameManager.playerCount;
        //spawnCooldown = 2f - (0.5f * playerCount);
        spawnCooldown = -0.5f * playerCount + 1.5f;

        // Start spawning diamonds
        StartCoroutine("RecursiveSpawner");
    }

	void Update () {

		if(gameManager.getState() == BaseGM.GAMESTATE.INGAME && this.transform.childCount == 0) {

			CreateDiamond();
		}
	}

    IEnumerator RecursiveSpawner()
    {
		if (gameManager.getState() == BaseGM.GAMESTATE.INGAME) {

			CreateDiamond();
			//yield return new WaitForSeconds (Random.Range (spawnCooldown * 0.5f, spawnCooldown * 1.5f));
			yield return new WaitForSeconds (spawnCooldown);
		}
		else {
			
			yield return new WaitForSeconds (0.5f);
		}
        
		StartCoroutine("RecursiveSpawner");
    }

	void CreateDiamond () {

		Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
		int diamondLayerMask = 1 << diamondLayer;
		int obstacleLayerMask = 1 << obstacleLayer;
		int combinedLayerMask = diamondLayerMask | obstacleLayerMask;
		Collider2D[] foundColliders;
		Vector3 itemLocation;

		do {

			float newX = Random.Range(spawnerBounds.min.x, spawnerBounds.max.x);
			float newY = Random.Range(spawnerBounds.min.y, spawnerBounds.max.y);
			itemLocation = new Vector3(newX, newY, 0);


			foundColliders = Physics2D.OverlapCircleAll(itemLocation, 1, combinedLayerMask);

		} while (foundColliders.Length > 0);

		GameObject newDiamond = Instantiate (itemPrefab, itemLocation, Quaternion.identity) as GameObject;
		newDiamond.transform.SetParent(this.transform);
	}
}

