using UnityEngine;
using System.Collections;

public class GameControllerParent : MonoBehaviour {

	protected GameObject[] playerArray;


	public GameObject[] GetActivePlayers () {

		return playerArray;
	}
}
