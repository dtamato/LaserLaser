using UnityEngine;
using System.Collections;

public class Paralysis : MonoBehaviour 
{
	[SerializeField] private GameObject playerOne;
	[SerializeField] private GameObject playerTwo;
	[SerializeField] private GameObject playerThree;
	[SerializeField] private GameObject playerFour;
	
	// Update is called once per frame
	void Update () 
	{
		if (this.gameObject == playerOne) {
			playerTwo.GetComponent<CannonTester> ().enabled = false;
			playerThree.GetComponent<CannonTester> ().enabled = false;
			playerFour.GetComponent<CannonTester> ().enabled =false;
		}
		if (this.gameObject == playerTwo) {
			playerOne.GetComponent<CannonTester> ().enabled = false;
			playerThree.GetComponent<CannonTester> ().enabled = false;
			playerFour.GetComponent<CannonTester> ().enabled =false;

		}
		if (this.gameObject == playerThree) {
			playerOne.GetComponent<CannonTester> ().enabled = false;
			playerTwo.GetComponent<CannonTester> ().enabled = false;
			playerFour.GetComponent<CannonTester> ().enabled =false;

		}
		if (this.gameObject == playerFour) {
			playerOne.GetComponent<CannonTester> ().enabled = false;
			playerTwo.GetComponent<CannonTester> ().enabled = false;
			playerThree.GetComponent<CannonTester> ().enabled =false;

		}
	}
}
