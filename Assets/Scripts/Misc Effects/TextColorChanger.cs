using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class TextColorChanger : MonoBehaviour {

	[SerializeField] float changeRate = 1;
	Text text;

	// Use this for initialization
	void Start () {
	
		text = this.GetComponent<Text> ();
		StartCoroutine (ChangeColor ());
	}

	IEnumerator ChangeColor () {

		yield return new WaitForSeconds (changeRate);
		text.color = new Color (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f));
		StartCoroutine (ChangeColor ());
	}
}
