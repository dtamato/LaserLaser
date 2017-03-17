using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class MenuLaser : MonoBehaviour {

	[SerializeField] GameObject menuPlayer;

	// Private variables
	Player rewiredPlayer;
	Rigidbody2D rb2d;


	void Awake()
	{
		rewiredPlayer = menuPlayer.GetComponentInChildren<MenuCannon> ().GetRewiredPlayer();

		rb2d = this.GetComponentInChildren<Rigidbody2D>();
		this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
		this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.CompareTag ("Boundary")) {

			rb2d.bodyType = RigidbodyType2D.Static; // HERE
			menuPlayer.transform.position = other.contacts [0].point;
			menuPlayer.transform.rotation = other.transform.rotation;
			menuPlayer.transform.GetChild (0).rotation = other.transform.rotation;
			this.transform.localPosition = new Vector3(0, 2f, 0);
			this.transform.GetComponent<SpriteRenderer> ().enabled = false;
			this.transform.GetComponent<TrailRenderer> ().enabled = false;
			menuPlayer.GetComponentInChildren<MenuCannon> ().SetNewBaseAngle ();
			menuPlayer.GetComponentInChildren<MenuCannon> ().SetInFlight (false);
		}
	}

	#region Rumble
	// Variable length full-intensity rumble function
	public IEnumerator Rumble(float duration)
	{
		rewiredPlayer = menuPlayer.GetComponentInChildren<MenuCannon> ().GetRewiredPlayer();

		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			if (!j.supportsVibration) continue;
			j.SetVibration(1.0f, 1.0f);
		}
		yield return new WaitForSeconds(duration);
		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			j.StopVibration();
		}
	}

	// Variable length low-intensity bump function
	public IEnumerator Bump(float duration)
	{
		rewiredPlayer = menuPlayer.GetComponentInChildren<MenuCannon> ().GetRewiredPlayer();

		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			if (!j.supportsVibration) continue;
			j.SetVibration(0.25f, 0.25f);
		}
		yield return new WaitForSeconds(duration);
		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			j.StopVibration();
		}
	}

	// Variable direction half-second rumble function 
	public IEnumerator DirectionalRumble(float leftIntensity, float rightIntensity)
	{
		rewiredPlayer = menuPlayer.GetComponentInChildren<MenuCannon> ().GetRewiredPlayer();

		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			if (!j.supportsVibration) continue;
			j.SetVibration(leftIntensity, rightIntensity);
		}
		yield return new WaitForSeconds(0.5f);
		foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
		{
			j.StopVibration();
		}
	}
	#endregion
}
