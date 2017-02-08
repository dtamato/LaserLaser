using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class TitleSceneController : MonoBehaviour {

	Player player;

	// Use this for initialization
	void Start () {
	
		player = ReInput.players.GetPlayer (0);
	}
	
	// Update is called once per frame
	void Update () {

		if (player.GetButton ("StartGame")) {

			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}
}
