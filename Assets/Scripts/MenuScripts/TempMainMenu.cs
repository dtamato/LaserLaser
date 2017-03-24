using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TempMainMenu : MonoBehaviour {

	public void LoadSceneByIndex(int index) {

		SceneManager.LoadScene (index);
	}

	public void LoadSceneByName (string name) {

		SceneManager.LoadScene (name);
	}
}
