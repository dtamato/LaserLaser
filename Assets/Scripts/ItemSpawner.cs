using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    float spawnCooldown = 1.0f;
    private BaseGM gameManager;
    void Start()
    {
        // Scale the spawn cooldown based on number of players.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        int playerCount = gameManager.playerCount;
        spawnCooldown = 1.0f - (0.05f * playerCount);
        // Start spawning diamonds
        StartCoroutine("RecursiveSpawner");
    }

    IEnumerator RecursiveSpawner()
    {
        StartCoroutine("SpawnItem");
        yield return new WaitForSeconds(Random.Range(spawnCooldown * 0.5f, spawnCooldown * 1.5f));
        StartCoroutine("RecursiveSpawner");
    }
    IEnumerator SpawnItem()
    {
        Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D>().bounds;
        float newX = Random.Range(spawnerBounds.min.x, spawnerBounds.max.x);
        float newY = Random.Range(spawnerBounds.min.y, spawnerBounds.max.y);
        Vector3 itemLocation = new Vector3(newX, newY, 0);
        GameObject newItem = Instantiate(itemPrefab, itemLocation, Quaternion.identity) as GameObject;
        newItem.transform.SetParent(this.transform);
        yield return new WaitForSeconds(spawnCooldown);
    }
}

