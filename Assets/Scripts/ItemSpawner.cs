using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    
    private BaseGM gameManager;
	private float spawnCooldown;

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

    IEnumerator RecursiveSpawner()
    {
		if (gameManager.getState() == BaseGM.GAMESTATE.INGAME) {
			Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D> ().bounds;
            //float itemRadius = itemPrefab.GetComponent<Collider2D>().bounds.extents.x;
            //Collider2D itemRadius = itemPrefab[0].GetComponent<Collider2D>();
            float newX = Random.Range (spawnerBounds.min.x, spawnerBounds.max.x);
			float newY = Random.Range (spawnerBounds.min.y, spawnerBounds.max.y);
			Vector3 itemLocation = new Vector3 (newX, newY, 0);
            Instantiate (itemPrefab, itemLocation, Quaternion.identity);
            //GameObject newItem = Instantiate(itemPrefab[Random.Range(0, itemPrefab.Length)], itemLocation, Quaternion.identity) as GameObject;
            //Collider2D[] diamondsFound = Physics2D.OverlapCircleAll(itemLocation, itemRadius.bounds.extents.x);
            //for (int i = 0; i < diamondsFound.Length; i++)
            //{
            //    Debug.Log("DiamondsFound: " + diamondsFound[i].name);
            //}

            /*if (Physics2D.OverlapCircle(itemLocation, itemRadius))
            {
                Debug.Log("No Spawn fo you");

                yield return null;
            }
            else
            {
                Instantiate(itemPrefab, itemLocation, Quaternion.identity);
            }*/

			//yield return new WaitForSeconds (Random.Range (spawnCooldown * 0.5f, spawnCooldown * 1.5f));
			yield return new WaitForSeconds (spawnCooldown);
		}
		else {
			
			yield return new WaitForSeconds (0.5f);
		}
        
		StartCoroutine("RecursiveSpawner");
    }
}

