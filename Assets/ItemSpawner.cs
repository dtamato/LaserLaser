using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ItemSpawner : MonoBehaviour {

	[SerializeField] GameObject itemPrefab;
	[SerializeField] float spawnCooldown = 2;

	// Use this for initialization
	void Start () {
	
		StopCoroutine(SpawnItem());
		StartCoroutine(SpawnItem());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnItem () {

		Bounds spawnerBounds = this.GetComponentInChildren<BoxCollider2D>().bounds;
		float newX = Random.Range(spawnerBounds.min.x, spawnerBounds.max.x);
		float newY = Random.Range(spawnerBounds.min.y, spawnerBounds.max.y);
		Vector3 itemLocation = new Vector3(newX, newY, 0);

		GameObject newItem = Instantiate(itemPrefab, itemLocation, Quaternion.identity) as GameObject;
		newItem.transform.SetParent(this.transform);

		yield return new WaitForSeconds(spawnCooldown);
		StartCoroutine(SpawnItem());
	}
}
