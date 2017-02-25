using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Light))]
public class BackgroundLight : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float moveSpeed = 1;
	[SerializeField] Color[] colorArray;

	[Header("References")]
	[SerializeField] Transform leftTarget;
	[SerializeField] Transform rightTarget;
	[SerializeField] Transform currentTarget;


	void Awake () {
		
		this.transform.position = currentTarget.position;
	}

	void Update () {

		if (currentTarget) {

			float distanceToTarget = Vector3.Distance (this.transform.position, currentTarget.position);

			if (distanceToTarget < 0.5f) {

				// Switch to other target
				currentTarget = (currentTarget == leftTarget) ? rightTarget : leftTarget;
				this.GetComponent<Light> ().color = colorArray [Random.Range (0, colorArray.Length)];
				this.GetComponent<Light> ().range = Random.Range (3, 5);
			}
			else {

				this.transform.position = Vector3.MoveTowards (this.transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
			}
		}
	}
}
