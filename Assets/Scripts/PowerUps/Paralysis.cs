using UnityEngine;
using System.Collections;

public class Paralysis : MonoBehaviour 
{
	[SerializeField] float paralysisTimer = 2;
	private GameObject[] player = new GameObject[4];

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("GameController").GetComponent<PowerupTestArenaController> ().GetActivePlayers();

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;
		for (int i = 0; i < 4; i++) {
			player [i].GetComponentInParent<Cannon> ().enabled = false;
		}

		other.GetComponentInParent<Cannon> ().enabled = true;
		StartCoroutine (DisableScript ());
			
	}

	IEnumerator DisableScript()
	{
		yield return new WaitForSeconds (paralysisTimer);
		for (int i = 0; i < 4; i++) {
			player [i].GetComponentInParent<Cannon> ().enabled = true;
		}
		Destroy (this.gameObject);
	}

}
