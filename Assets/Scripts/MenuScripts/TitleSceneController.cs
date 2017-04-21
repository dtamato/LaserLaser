using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TitleSceneController : MonoBehaviour {

	[SerializeField] GameObject pressStartObject;
	[SerializeField] float pressStartFlickerTime = 1;

	[Header("Demo")]
	[SerializeField] bool canShowDemo = true;
	[SerializeField] float secondsUntilDemoPlays = 3;
	[SerializeField] Renderer movieRenderer;
	[SerializeField] FadeCameraOverlay cameraOverlay;

	float flickerTimer;

	void Start () {

		flickerTimer = pressStartFlickerTime;
		movieRenderer.gameObject.SetActive(false);
		if(canShowDemo) { StartCoroutine(LoadDemo()); }
	}

	IEnumerator LoadDemo () {

		yield return new WaitForSeconds(secondsUntilDemoPlays);

		movieRenderer.gameObject.SetActive(true);
		MovieTexture movie = (MovieTexture)movieRenderer.material.mainTexture;
		movie.Play();
		movie.loop = true;
	}

	void Update () {

		if(flickerTimer > 0) {

			flickerTimer -= Time.deltaTime;
		}
		else {

			pressStartObject.SetActive(!pressStartObject.activeSelf);
			flickerTimer = pressStartFlickerTime;
		}

		if (Input.anyKeyDown) {

			if(movieRenderer.gameObject.activeSelf) {

				movieRenderer.gameObject.SetActive(false);
			}
			else {

				cameraOverlay.FadeToBlack();
				this.GetComponent<AudioSource>().Play();
			}
		}

		if(cameraOverlay.GetComponent<Image>().color.a >= 1) {

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}