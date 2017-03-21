using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

[DisallowMultipleComponent]
public class TitleSceneController : MonoBehaviour {

	[SerializeField] GameObject pressStartObject;
	[SerializeField] float pressStartFlickerTime = 1;
	[SerializeField] float secondsUntilDemoPlays = 3;
	[SerializeField] Renderer movieRenderer;

	float flickerTimer;

	void Start () {

		flickerTimer = pressStartFlickerTime;
		movieRenderer.gameObject.SetActive(false);
		StartCoroutine(LoadDemo());
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
				
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
			}
		}
	}
}
