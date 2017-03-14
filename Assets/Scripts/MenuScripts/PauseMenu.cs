using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PauseMenu : MonoBehaviour {

	[SerializeField] Image borderImage;
	[SerializeField] Text pauseText;


	public void PauseGame (GameObject playerPausing) {

		Color playerColor = playerPausing.GetComponentInChildren<SpriteRenderer>().color;
		borderImage.color = playerColor;
		pauseText.color = playerColor;

		Time.timeScale = 0;
	}

	public void ResumeGame () {

		Time.timeScale = 1;
		this.gameObject.SetActive(false);
	}

	public void RestartGame () {

		Time.timeScale = 1;
		this.gameObject.SetActive(false);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void QuitGame () {

		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}
}
