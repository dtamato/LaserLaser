using UnityEngine;
using System.Collections;

public class Slow : MonoBehaviour 
{
	[SerializeField] float slowSpeed = 0.5f;
	[SerializeField] float slowTimer = 3;
	private float[] rotSpeed = new float[4];
	private float collidingPlayerRotSpeed;
	private GameObject[] player = new GameObject[4];

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PowerupTestArenaController> ().GetActivePlayers ();

	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		for (int i = 0; i < 4; i++) {
			rotSpeed[i] = player [i].GetComponentInParent<Cannon> ().GetBaseSpeed ();
		}
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;
		collidingPlayerRotSpeed = other.GetComponentInParent<Cannon> ().GetBaseSpeed ();
		for (int i = 0; i < 4; i++) {
			player [i].GetComponentInParent<Cannon> ().ModifyRotationSpeed(slowSpeed);
		}
		other.GetComponentInParent<Cannon> ().ModifyRotationSpeed(collidingPlayerRotSpeed);
		StartCoroutine (ReturnBaseSpeed ());
	}

	IEnumerator ReturnBaseSpeed()
	{
		yield return new WaitForSeconds (slowTimer);
		for (int i = 0; i < 4; i++) {
			player [i].GetComponentInParent<Cannon> ().ModifyRotationSpeed(rotSpeed[i]);
		}
		Destroy (this.gameObject);
	}
}
