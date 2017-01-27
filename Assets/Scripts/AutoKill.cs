using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class AutoKill : MonoBehaviour {

	void Start () {

		Destroy (this.gameObject, 5);
	}
}
