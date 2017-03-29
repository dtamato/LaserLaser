using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FadeCameraOverlay : MonoBehaviour {

	Image cameraOverlay;
	bool fadingOut = false;
	bool fadingToBlack = false;

	void Awake () {

		cameraOverlay = this.GetComponent<Image>();
		cameraOverlay.color = Color.black;
		fadingOut = true;
	}

	void Update () {

		if(fadingOut) {
			
			float newAlpha = cameraOverlay.color.a - Time.deltaTime;
			cameraOverlay.color = new Color(0, 0, 0, newAlpha);

			if(newAlpha <= 0) { fadingOut = false; }
		}
		else if(fadingToBlack) {

			float newAlpha = cameraOverlay.color.a + Time.deltaTime;
			cameraOverlay.color = new Color(0, 0, 0, newAlpha);

			if(newAlpha >= 1) { fadingToBlack = false; }
		}
	}

	public void FadeOut () {

		fadingOut = true;
	}

	public void FadeToBlack () {

		fadingToBlack = true;
	}
}
