using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ThreeKeysSpeed : MonoBehaviour {

	[SerializeField] GameObject resetTrigger;


	void OnTriggerEnter2D(Collider2D other) {


		if(other.CompareTag("Player")) {

			resetTrigger.SetActive(true);
		}
	}
}
