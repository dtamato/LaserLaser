using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SinglePlayerCamera : MonoBehaviour {

	[SerializeField] GameObject door;

	void Start () {

		this.GetComponent<Animator>().SetTrigger("Start");
	}

	public void ShowEnemy () {

		this.GetComponent<Animator>().enabled = true;
		this.GetComponent<Animator>().SetTrigger("ShowEnemy");
	}

	public void ShowDoor () {

		this.GetComponent<Animator>().enabled = true;
		this.GetComponent<Animator>().SetTrigger("ShowDoor");
	}

	public void EnableSmoothFollow () {

		this.GetComponent<CameraSmoothFollow>().enabled = true;
		this.GetComponent<Animator>().enabled = false;
	}

	public void DisableSmoothFollow () {
		
		this.GetComponent<CameraSmoothFollow>().enabled = false;
		this.GetComponent<Animator>().enabled = true;
	}

	public void UnlockDoor () {

		door.SetActive(false);
	}
}
