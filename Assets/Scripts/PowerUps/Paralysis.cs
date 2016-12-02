using UnityEngine;
using System.Collections;

public class Paralysis : MonoBehaviour 
{
	public GameObject playerOne;
	public GameObject playerTwo;
	public GameObject playerThree;
	public GameObject playerFour;
	
	// Update is called once per frame
	void Update () 
	{
		if (this.gameObject == playerOne) {
			playerTwo.GetComponent<Cannon> ().enabled = false;
			playerThree.GetComponent<Cannon> ().enabled = false;
			playerFour.GetComponent<Cannon> ().enabled =false;
		}
		if (this.gameObject == playerTwo) {
			playerOne.GetComponent<Cannon> ().enabled = false;
			playerThree.GetComponent<Cannon> ().enabled = false;
			playerFour.GetComponent<Cannon> ().enabled =false;

		}
		if (this.gameObject == playerThree) {
			playerOne.GetComponent<Cannon> ().enabled = false;
			playerTwo.GetComponent<Cannon> ().enabled = false;
			playerFour.GetComponent<Cannon> ().enabled =false;

		}
		if (this.gameObject == playerFour) {
			playerOne.GetComponent<Cannon> ().enabled = false;
			playerTwo.GetComponent<Cannon> ().enabled = false;
			playerThree.GetComponent<Cannon> ().enabled =false;

		}
	}
}
