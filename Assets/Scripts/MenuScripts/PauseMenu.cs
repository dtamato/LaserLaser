using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PauseMenu : MonoBehaviour {

	[SerializeField] Image borderImage;
	[SerializeField] Text pauseText;

	GameObject playerPausing;
    private BaseGM gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
    }

	public void PauseGame (GameObject player) {

		playerPausing = player;
		Color playerColor = playerPausing.GetComponentInChildren<SpriteRenderer>().color;
		borderImage.color = playerColor;
		pauseText.color = playerColor;

		Button firstButton = this.GetComponentInChildren<Button>();

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
	}

	public void ResumeGame () {

		//playerPausing.GetComponentInChildren<Cannon>().SetIsPaused(false);
		this.gameObject.SetActive(false);
        gameManager.EnablePlayerControllers();
    }

	public void RestartGame () {

		/*this.gameObject.SetActive(false);
        gameManager.EnablePlayerControllers();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);*/
	    Debug.Log("Breaks Game");
	}

	public void QuitGame () {

		Destroy(GameObject.Find("Rewired Input Manager"));
		Destroy(GameObject.FindGameObjectWithTag("GameManager").gameObject);
		SceneManager.LoadScene(0);
	}

	public void NextButton ()
	{
		Button currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
		Selectable nextButton = currentButton.FindSelectableOnDown();
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
	}
}
