using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuStrobeText : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float colorChangeDelay = 0.1f;

	[Header("References")]
	[SerializeField] Text[] textToBlink;
	[SerializeField] Color[] colors;

	void Awake () {

		StartCoroutine(ToggleColors());
	}

	IEnumerator ToggleColors () {

		while (true) {

			yield return new WaitForSeconds (colorChangeDelay);

			textToBlink [0].color = textToBlink[0].color == colors[0] ? colors[1] : colors [0];
			textToBlink [1].color = textToBlink[1].color == colors[0] ? colors[1] : colors [0];
		}
	}
}
