using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class LogoSceneController : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float changeColorDelay = 0.1f;
	[SerializeField] Color[] colorOptions;
	[SerializeField] float sceneLoadDelay = 3;

	[Header("References")]
	[SerializeField] SpriteRenderer outlineSprite;
	[SerializeField] SpriteRenderer faceSprite;

	void Start () {

		StartCoroutine (ChangeColor ());
		StartCoroutine (DelayLoadScene ());
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

	IEnumerator DelayLoadScene () {

		yield return new WaitForSeconds (sceneLoadDelay);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}
}
