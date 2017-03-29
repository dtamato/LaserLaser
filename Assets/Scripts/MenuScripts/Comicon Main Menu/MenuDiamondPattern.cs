using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MenuDiamondPattern : MonoBehaviour {

	[SerializeField] GameObject menuDiamondPrefab;

	GameObject[] diamonds;
	Vector3[] diamondPositions;
	Quaternion[] diamondRotations;
	bool playerIn;

	void Awake () {

		StoreDiamondData ();
		StartCoroutine (RestoreMissingDiamonds ());
	}

	void StoreDiamondData () {

		diamonds = new GameObject[this.transform.childCount];
		diamondPositions = new Vector3[diamonds.Length];
		diamondRotations = new Quaternion[diamonds.Length];

		for (int i = 0; i < this.transform.childCount; i++) {

			diamonds [i] = this.transform.GetChild (i).gameObject;
			diamondPositions [i] = diamonds [i].transform.localPosition;
			diamondRotations [i] = diamonds [i].transform.localRotation;
		}
	}

	IEnumerator RestoreMissingDiamonds () {
			
		if (!playerIn) {
			
			for (int i = 0; i < diamonds.Length; i++) {

				if (diamonds [i] == null) {

					GameObject newDiamond = Instantiate (menuDiamondPrefab, this.transform) as GameObject;
					newDiamond.transform.localPosition = diamondPositions [i];
					newDiamond.transform.localRotation = diamondRotations [i];
					diamonds [i] = newDiamond;
				}
			}
		}

		yield return new WaitForSeconds (5);

		StartCoroutine (RestoreMissingDiamonds ());
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.CompareTag ("Cannon")) {

			playerIn = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {

		if (other.CompareTag ("Cannon")) {

			playerIn = false;
		}
	}
}