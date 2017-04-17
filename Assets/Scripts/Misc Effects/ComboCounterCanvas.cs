using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ComboCounterCanvas : MonoBehaviour {

	Text comboText;


	void Awake () {

		comboText = this.GetComponentInChildren<Text>();
	}

	void Update () {

		if(comboText.color.a > 0) {

			float newAlpha = comboText.color.a - 2 * Time.deltaTime;
			comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, newAlpha);
		}

		if(comboText.color.a <= 0) {

			Destroy(this.gameObject);
		}
	}

	public void SetText (int comboCount) {

		comboText.text = "+" + comboCount;
	}
}
