using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class DiamondCollected : MonoBehaviour {

	void Awake () {
		
		Destroy (this.gameObject, 1);
	}

	public void ChangeColor (Color newColor) {

		this.GetComponent<ParticleSystem> ().startColor = newColor;
		this.GetComponent<ParticleSystem> ().Play ();
	}
}
