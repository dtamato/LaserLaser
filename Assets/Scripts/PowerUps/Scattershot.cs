using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This is the scattershot powerUp script.
public class Scattershot : MonoBehaviour 
{
	[SerializeField] float scatterTimer = 10;
	public GameObject scatterObject;
	private GameObject[] scatterBall = new GameObject[4];

	void Start()
	{
		StartCoroutine (DisablePowerUp ());
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// Make Scattershot Icon disappear.
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;
		scatterTimer += 10; // adds 10 seconds to the destroy timer.

		//  Instantiate 4 balls & Change the color of each ball & Set the ID of each ball
		for (int i = 0; i < 4; i++) {
			scatterBall [i] = Instantiate(scatterObject, other.transform.position, other.transform.rotation);
			scatterBall [i].GetComponent<SpriteRenderer> ().color = other.gameObject.GetComponent<SpriteRenderer> ().color;
			scatterBall [i].transform.parent = other.gameObject.transform;
		}

		// Change the rotation of each instantiated object. Making them launch in different directions (NE,NW, SE, SW).
		scatterBall[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
		scatterBall[1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 135));
		scatterBall[2].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 225));
		scatterBall[3].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 315));

	}

	IEnumerator DisablePowerUp ()
	{
		yield return new WaitForSeconds (scatterTimer);
		Destroy (this.gameObject);
	}
}
