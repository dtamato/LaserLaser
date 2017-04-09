using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraSmoothFollow : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float dampTime = 0.15f;
	[SerializeField] float minX = 0;
	[SerializeField] float maxX = 0;
	[SerializeField] float minY = 0;
	[SerializeField] float maxY = 0;

	[Header("References")]
	[SerializeField] Transform target;

	Vector3 camVelocity = Vector3.zero;


	void Start () {

		if (target == null) {
			Debug.LogWarning ("Smooth follow target not set!");
		}
	}

	void LateUpdate () {

		if (target) {

			Vector3 trackingTarget = new Vector3(target.transform.position.x, target.transform.position.y + 2, this.transform.position.z);
			float clampedX = Mathf.Clamp (trackingTarget.x, minX, maxX);
			float clampedY = Mathf.Clamp (trackingTarget.y, minY, maxY);
			trackingTarget = new Vector3 (clampedX, clampedY, this.transform.position.z);
			transform.position = Vector3.SmoothDamp(transform.position, trackingTarget, ref camVelocity, dampTime);		
		}
	}

	public void SetTarget(GameObject newTarget) {

		target = newTarget.transform;
	}

	public void RemoveTarget() {

		target = null;
	}

	public void ChangeClamps(float newMinx, float newMaxX, float newMinY, float newMaxY) {

		minX = newMinx;
		maxX = newMaxX;
		minY = newMinY;
		maxY = newMaxY;
	}
}
