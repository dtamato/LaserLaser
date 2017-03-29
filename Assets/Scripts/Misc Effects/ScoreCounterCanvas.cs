using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ScoreCounterCanvas : MonoBehaviour {

	Text scoreText;

	void Awake () {

		scoreText = this.GetComponentInChildren<Text>();
	}

	void Update () {

		if(scoreText.color.a > 0) {

			float newAlpha = scoreText.color.a - Time.deltaTime;

			if(newAlpha <= 0) {

				Destroy(this.gameObject);
			}
			else {

				this.transform.Translate(Vector3.up * Time.deltaTime);
				scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, newAlpha);
			}
		}
	}

	public void SetText(string score) {

		scoreText.text = score;
	}

	public void SetText(int score) {

		scoreText.text = score.ToString();
	}
}
