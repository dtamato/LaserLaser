using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlast : MonoBehaviour 
{
	// Timer for the icon.
	[SerializeField] float areaTimer = 5;
	[SerializeField] GameObject areaBlaster;

	void Start()
	{
		StartCoroutine (DisablePowerUp ());
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		// Make the Area Blast Icon Disappear.
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = false;
		// Extends the destroy timer.
		areaTimer += 5;

		// Instantiate a Circle with area of effect.
		GameObject newAreaBlaster = Instantiate(areaBlaster, other.transform.position, other.transform.rotation) as GameObject;
		// Change the color of the circle to the players color.
		newAreaBlaster.GetComponent<SpriteRenderer> ().color = other.gameObject.GetComponent<SpriteRenderer> ().color;
		// Set the player id & team  to the player that collided with this powerup.
		newAreaBlaster.GetComponent<Blaster> ().playerNum = other.gameObject.GetComponent<Laser> ().myPlayerID;
		newAreaBlaster.GetComponent<Blaster> ().teamNum = other.gameObject.GetComponent<Laser> ().myTeam;
	}

	// Disable the power up after set time.
	IEnumerator DisablePowerUp()
	{
		yield return new WaitForSeconds (areaTimer);
		Destroy (this.gameObject);
	}
}
