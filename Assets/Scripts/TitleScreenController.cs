using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TitleScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (ReInput.players.GetPlayer (0).GetButtonDown("Fire")) {

			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}
}
