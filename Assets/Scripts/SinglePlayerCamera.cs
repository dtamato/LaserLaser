using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SinglePlayerCamera : MonoBehaviour {


	public void DisableAnimator () {

		this.GetComponent<Animator>().enabled = false;
		this.GetComponent<CameraSmoothFollow>().enabled = true;
	}
}
