using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class EndDiamond : MonoBehaviour {

	[SerializeField] Image cameraOverlay;
	[SerializeField] Text endText;

	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Player")) {

			this.GetComponent<Collider2D>().enabled = false;
			this.GetComponent<SpriteRenderer>().enabled = false;
			StartCoroutine(EndGame());
		}
	}

	IEnumerator EndGame () {

		cameraOverlay.color = Color.clear;
		endText.color = Color.clear;

		while(cameraOverlay.color.a < 1) {

			float newAlpha = cameraOverlay.color.a + Time.deltaTime;
			cameraOverlay.color = new Color(0, 0, 0, newAlpha);

			float newTextAlpha = endText.color.a + Time.deltaTime;
			endText.color = new Color(1, 1, 1, newAlpha);

			yield return null;
		}

		Destroy(GameObject.Find("Rewired Input Manager"));
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		yield return null;
	}
}
