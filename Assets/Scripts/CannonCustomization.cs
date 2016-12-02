using Rewired;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(Cannon))]
public class CannonCustomization : MonoBehaviour {

	[SerializeField] float rotationSpeedIncrement = 0.5f;

	Cannon cannon;

	// External references
	GameControllerParent gameController;
	Player rewiredPlayer;

	void Awake () {

		cannon = this.GetComponentInChildren<Cannon> ();
		rewiredPlayer = ReInput.players.GetPlayer (cannon.GetPlayerID ());
		//gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameControllerParent> ();
	}

	void Update () {

		ProcessInputs ();
	}

	void ProcessInputs () {

		ChangeRotationSpeed ();
		ChangeColor();
	}

	void ChangeRotationSpeed () {

		if (rewiredPlayer.GetButtonDown ("IncreaseRotationSpeed")) {

			cannon.ChangeRotationSpeed (rotationSpeedIncrement);
		}
		else if (rewiredPlayer.GetButtonDown ("DecreaseRotationSpeed")) {

			cannon.ChangeRotationSpeed (-rotationSpeedIncrement);
		}
	}

	void ChangeColor () {

		if (rewiredPlayer.GetButtonDown ("NextColor")) {

			cannon.ChangeColor ();
		}
		else if (rewiredPlayer.GetButtonDown ("LastColor")) {
			
			cannon.ChangeColor ();
		}
	}
}
