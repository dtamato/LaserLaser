using UnityEngine;

[DisallowMultipleComponent]
public class PowerupTestArenaController : GameControllerParent {


	void Awake () {

		playerArray = GameObject.FindGameObjectsWithTag ("Cannon");
	}
}
