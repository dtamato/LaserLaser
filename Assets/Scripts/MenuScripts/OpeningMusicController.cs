using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class OpeningMusicController : MonoBehaviour {

	[SerializeField] int gameBuildIndex = 3;

	static OpeningMusicController instance;

	void Awake () {

		if (instance == null) {

			instance = this;
		}
		else {

			Destroy (this.gameObject);
		}

		DontDestroyOnLoad (this.gameObject);
	}

	void OnEnable() {

		// Subscribe delegate
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable() {

		// Unsubscribe delegate
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		
		//Debug.Log ("Level Loaded: " + SceneManager.GetActiveScene ().buildIndex);
		if (SceneManager.GetActiveScene ().buildIndex == gameBuildIndex) {

			Destroy (this.gameObject);
		}
	}
}
