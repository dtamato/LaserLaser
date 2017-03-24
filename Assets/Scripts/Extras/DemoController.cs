using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DemoController : MonoBehaviour {

	[SerializeField] float secondsUntilDemoPlays = 5;
	[SerializeField] Renderer movieRenderer;
	[SerializeField] GameObject canvasGameObject;

	void Start () {

		movieRenderer.gameObject.SetActive(false);
		StartCoroutine(LoadDemo());
	}

	IEnumerator LoadDemo () {

		yield return new WaitForSeconds(secondsUntilDemoPlays);

		canvasGameObject.SetActive(false);
		movieRenderer.gameObject.SetActive(true);
		MovieTexture movie = (MovieTexture)movieRenderer.material.mainTexture;
		movie.Play();
		movie.loop = true;
	}

	void Update () {

		if(Input.anyKeyDown) {

			if(movieRenderer.gameObject.activeSelf) {

				movieRenderer.gameObject.SetActive(false);
				canvasGameObject.SetActive(true);

				StopAllCoroutines();
				StartCoroutine(LoadDemo());
			}
		}
	}
}
