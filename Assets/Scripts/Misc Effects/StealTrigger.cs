using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class StealTrigger : MonoBehaviour {

	[SerializeField] GameObject trickshotCanvasPrefab;

	Color textColor;


	void Awake () {

		Destroy(this.gameObject, 0.5f);
	}

	void OnTriggerEnter2D (Collider2D other) {

		if(other.CompareTag("Player") && other.GetComponentInChildren<SpriteRenderer>().color != textColor) {

			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, this.transform.position, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponentInChildren<Text>().color = textColor;
			newTrickshotCanvas.GetComponentInChildren<Text>().fontSize = 80;
			newTrickshotCanvas.GetComponentInChildren<Text>().text = "TOO SLOW!";
			newTrickshotCanvas.transform.localScale = Vector3.one;
			newTrickshotCanvas.GetComponent<Animator>().enabled = false;
			Destroy(this.gameObject);
		}
	}

	public void SetColor(Color newTextColor) {

		textColor = newTextColor;
	}
}
