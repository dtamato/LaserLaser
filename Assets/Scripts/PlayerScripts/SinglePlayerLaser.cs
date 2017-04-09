using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class SinglePlayerLaser : MonoBehaviour {

	// References
	BaseGM gameManager;
	GameObject cannon;
	Rigidbody2D rb2d;
	Player rewiredPlayer;


	void Awake()
	{
		cannon = this.transform.parent.gameObject;
		rb2d = this.GetComponentInChildren<Rigidbody2D>();
		this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
		this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.CompareTag ("Boundary")) {

			rb2d.bodyType = RigidbodyType2D.Static;
			rb2d.GetComponent<Collider2D>().isTrigger = true;
			cannon.transform.position = other.contacts [0].point;
			cannon.transform.rotation = other.transform.rotation;
			this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
			this.transform.GetComponent<SpriteRenderer> ().enabled = false;
			this.transform.GetComponent<TrailRenderer> ().enabled = false;
			cannon.GetComponentInChildren<SinglePlayerCannon> ().SetNewBaseAngle ();
			cannon.GetComponent<SinglePlayerCannon> ().inFlight = false;
		}
		else if(other.transform.CompareTag("Spike")) {

			StartCoroutine(HitSpike());
		}
	}

	IEnumerator HitSpike () {

		StartCoroutine(Rumble(0.25f));

		if(this.GetComponent<SpriteRenderer>().color != Color.red) {
			
			Color originalColor = this.GetComponent<SpriteRenderer>().color;
			this.GetComponent<SpriteRenderer>().color = Color.red;

			yield return new WaitForSeconds(0.5f);

			this.GetComponent<SpriteRenderer>().color = originalColor;
		}

		yield return null;
	}

	#region Rumble
	// Variable length full-intensity rumble function
	public IEnumerator Rumble(float duration)
	{
		rewiredPlayer = ReInput.players.GetPlayer(0);

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
		rewiredPlayer = ReInput.players.GetPlayer(0);

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
		rewiredPlayer = ReInput.players.GetPlayer(0);

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
