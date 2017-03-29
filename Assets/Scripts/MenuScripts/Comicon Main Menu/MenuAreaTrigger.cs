using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

[DisallowMultipleComponent]
public class MenuAreaTrigger : MonoBehaviour {

	[SerializeField] Color highlightColor = Color.red;
	[SerializeField] Text gameTypeText;
	[SerializeField] Text gameTypeDescriptionText;
	[SerializeField] string sceneToLoad;

	Player rewiredPlayer;
	SpriteRenderer spriteRenderer;
	bool playerIn = false;

	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer (0);
		spriteRenderer = this.GetComponentInChildren<SpriteRenderer> ();
		//gameTypeDescriptionText.gameObject.SetActive (false);
	}

	void Update () {

		if(playerIn && rewiredPlayer.GetButton("StartGame")) {

			Debug.Log ("Loading: " + sceneToLoad);
			//SceneManager.LoadScene (sceneToLoad);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		
		if (other.CompareTag ("Cannon")) {

			spriteRenderer.color = highlightColor;
			gameTypeText.color = highlightColor;
			//gameTypeDescriptionText.gameObject.SetActive (true);
			gameTypeDescriptionText.color = highlightColor;
			playerIn = true;
		}
	}

	void OnTriggerExit2D (Collider2D other) {

		if (other.CompareTag ("Cannon")) {

			spriteRenderer.color = Color.white;
			gameTypeText.color = Color.white;
			//gameTypeDescriptionText.gameObject.SetActive (false);
			gameTypeDescriptionText.color = Color.white;
			playerIn = false;
		}
	}
}
