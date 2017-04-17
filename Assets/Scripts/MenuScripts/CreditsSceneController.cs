using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class CreditsSceneController : MonoBehaviour {

	[SerializeField] Transform namesParent;
	[SerializeField] Image cameraOverlay;
	GameObject[] namesArray;
	bool finished = false;

	void Awake () {

		FillNamesArray();
		StartCoroutine(ShowNames());
	}

	void FillNamesArray () {

		namesArray = new GameObject[namesParent.childCount];

		for(int i = 0; i < namesArray.Length; i++) {

			namesArray[i] = namesParent.GetChild(i).gameObject;
			namesArray[i].GetComponent<Text>().color = Color.clear;
		}
	}

	IEnumerator ShowNames () {

		for(int i = 0; i < namesArray.Length; i++) {

			Text currentNamesText = namesArray[i].GetComponent<Text>();

			while(currentNamesText.color.a < 1) {

				float newAlpha = currentNamesText.color.a + 7 * Time.deltaTime;
				currentNamesText.color = new Color(1, 1, 1, newAlpha);
				yield return new WaitForSeconds(0.1f);
			}
		}
			
		StartCoroutine(LoadStartScene());
	}

	void Update () {

		if(Input.anyKeyDown && Time.timeSinceLevelLoad > 2) {

			RevealAllNames();
		}
	}

	void RevealAllNames () {

		for(int i = 0; i < namesArray.Length; i++) {

			namesArray[i].GetComponent<Text>().color = Color.white;
		}

		StartCoroutine(LoadStartScene());
	}

	IEnumerator LoadStartScene () {

		yield return new WaitForSeconds(1);

		// Fade to black
		while (cameraOverlay.color.a < 1) {

			float newAlpha = cameraOverlay.color.a + Time.deltaTime;
			cameraOverlay.color = new Color (0, 0, 0, newAlpha);
			yield return null;
		}

		if(!finished) {

			finished = true;
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>().returnToMenu();
		}
	}
}
