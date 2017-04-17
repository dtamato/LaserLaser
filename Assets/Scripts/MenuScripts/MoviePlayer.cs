using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MoviePlayer : MonoBehaviour {

	[SerializeField] float playDelay = 1;
	Renderer movieRenderer;


	void Awake () {

		movieRenderer = this.GetComponent<Renderer>();
		StartCoroutine(DelayStartMovie());
	}

	IEnumerator DelayStartMovie () {

		yield return new WaitForSeconds(playDelay);
		MovieTexture movie = (MovieTexture)movieRenderer.material.mainTexture;
	    if (movie != null)
	    {
		    movie.Play();
            movie.loop = true;
        }
	    else
	    {
	        Debug.Log("There is no movie to play!");
	    }
	}
}
