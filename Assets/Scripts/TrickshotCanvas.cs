using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TrickshotCanvas : MonoBehaviour {

	[SerializeField] float secondsBeforeFading = 0.5f;
	Text trickshotText;
	float timer;

	void Awake () {

		trickshotText = this.GetComponentInChildren<Text>();
		timer = 0;
	}

	void Update () {

		timer += Time.deltaTime;

		if(timer > secondsBeforeFading) {

			float newAlpha = trickshotText.color.a - Time.deltaTime;

			if(newAlpha <= 0) {

				Destroy(this.gameObject);
			}
			else {

				this.transform.Translate(Vector3.up * Time.deltaTime);
				trickshotText.color = new Color(trickshotText.color.r, trickshotText.color.g, trickshotText.color.b, newAlpha);
			}
		}
	}

	public void SetText(string newText) {

		trickshotText.text = newText;
	}
}
