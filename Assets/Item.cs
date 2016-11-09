using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Destroy(this.gameObject, 10);
	}

	void Update () {

		Color currentColor = this.GetComponentInChildren<SpriteRenderer>().color;
		this.GetComponentInChildren<SpriteRenderer>().color = currentColor - new Color(0, 0, 0, 0.1f * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other) {

		//Destroy(this.gameObject);
	}
}
