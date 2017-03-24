using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class LogoSceneController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float changeColorDelay = 0.1f;
	[SerializeField] Color[] colorOptions;
	[SerializeField] float fadeSpeed = 0.1f;

	[Header("References")]
	[SerializeField] SpriteRenderer outlineSprite;
	[SerializeField] SpriteRenderer faceSprite;
	[SerializeField] SpriteRenderer cameraOverlay;

	void Start () {

		StartCoroutine (FadeScreen ());
		StartCoroutine (ChangeColor ());
	}

	IEnumerator FadeScreen () {

		cameraOverlay.gameObject.SetActive (true);
		cameraOverlay.color = Color.black;

		yield return new WaitForSeconds (1);

		// Fade in
		while (cameraOverlay.color.a > 0) {

			float newAlpha = cameraOverlay.color.a - fadeSpeed * Time.deltaTime;
			cameraOverlay.color = new Color (0, 0, 0, newAlpha);
			yield return null;
		}

		// Pause
		yield return new WaitForSeconds (1);

		// Fade out
		while (cameraOverlay.color.a < 1) {

			float newAlpha = cameraOverlay.color.a + fadeSpeed * Time.deltaTime;
			cameraOverlay.color = new Color (0, 0, 0, newAlpha);
			yield return null;
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}

	IEnumerator ChangeColor () {

		Color newColor;

		do {

			newColor = colorOptions [Random.Range (0, colorOptions.Length)];

		} while (newColor == outlineSprite.color);

		outlineSprite.color = newColor;
		faceSprite.color = newColor;

		yield return new WaitForSeconds (changeColorDelay);

		StartCoroutine (ChangeColor ());
	}
}
