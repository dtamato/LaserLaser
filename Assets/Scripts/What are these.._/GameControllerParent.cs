using UnityEngine;
using System.Collections;

public class GameControllerParent : MonoBehaviour {

	protected GameObject[] playerArray = new GameObject[4];


	public GameObject[] GetActivePlayers () {

		return playerArray;
	}
}
