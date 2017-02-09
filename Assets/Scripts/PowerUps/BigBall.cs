using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBall : MonoBehaviour 
{
	[SerializeField] float bigTimer = 4;
	[SerializeField] float size = 1;
	private GameObject temp;
	private GameObject[] players = new GameObject[4];

	// Use this for initialization
	void Start () 
	{
		players = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<BaseGM> ().GetActivePlayers ();
		
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		// Make BigBall Icon disappear.
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;

		// Make the player that collided to the BigBall, have a bigger ball.
		temp = other.gameObject;
		other.transform.localScale += new Vector3 (size, size, size);
		StartCoroutine (DisablePowerUp ());
	}

	IEnumerator DisablePowerUp ()
	{
		yield return new WaitForSeconds (bigTimer);
		temp.transform.localScale = Vector3.one;
		Destroy (this.gameObject);
	}
}
