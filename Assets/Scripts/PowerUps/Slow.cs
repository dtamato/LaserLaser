using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Slow : MonoBehaviour 
{
	[SerializeField] float slowPercentage = 0.5f;
	[SerializeField] float slowTimer = 3;
	private float[] rotSpeed = new float[4];
	private float collidingPlayerRotSpeed;
	private GameObject[] players = new GameObject[4];

	// Use this for initialization
	void Start () 
	{
		players = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<BaseGM> ().GetActivePlayers ();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		for (int i = 0; i < players.Length; i++) {
			
			rotSpeed[i] = players [i].GetComponentInParent<Cannon> ().GetRotationSpeed ();
		}

		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;

		collidingPlayerRotSpeed = other.GetComponentInParent<Cannon> ().GetRotationSpeed ();

		for (int i = 0; i < players.Length; i++) {

			float newSpeed = slowPercentage * players [i].GetComponentInParent<Cannon> ().GetRotationSpeed ();
			players [i].GetComponentInParent<Cannon> ().SetRotationSpeed(newSpeed);
		}

		other.GetComponentInParent<Cannon> ().SetRotationSpeed(collidingPlayerRotSpeed);
		StartCoroutine (ReturnBaseSpeed ());
	}

	IEnumerator ReturnBaseSpeed()
	{
		yield return new WaitForSeconds (slowTimer);

		for (int i = 0; i < players.Length; i++) {
			
			players [i].GetComponentInParent<Cannon> ().SetRotationSpeed(rotSpeed[i]);
		}

		Destroy (this.gameObject);
	}
}
