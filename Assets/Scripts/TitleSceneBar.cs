using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class TitleSceneBar : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float startDelay = 0;
	[SerializeField] float moveSpeed = 1;

	[Header("References")]
	[SerializeField] GameObject colorBar;
	[SerializeField] Transform startPointTransform;
	[SerializeField] Transform endPointTransform;

	// Privates
	bool movingOut = true;

	void Start () {

		moveSpeed = Random.Range(moveSpeed * 0.5f, moveSpeed * 1.5f);
		colorBar.transform.localPosition = startPointTransform.localPosition;
	}

	void Update () {

		if(Time.time > startDelay) {

			if (movingOut) {
				
				if (Vector3.Distance(colorBar.transform.localPosition, endPointTransform.localPosition) > 0.1f) {

					colorBar.transform.localPosition = Vector3.Slerp (colorBar.transform.localPosition, endPointTransform.localPosition, moveSpeed * Time.deltaTime);
				}
				else {

					movingOut = !movingOut;
					moveSpeed = Random.Range(moveSpeed * 0.5f, moveSpeed * 1.25f);
				}
			}
			else {

				if (Vector3.Distance(colorBar.transform.localPosition, startPointTransform.localPosition) > 0.1f) {

					colorBar.transform.localPosition = Vector3.Slerp (colorBar.transform.localPosition, startPointTransform.localPosition, moveSpeed * Time.deltaTime);
				}
				else {

					movingOut = !movingOut;
					moveSpeed = Random.Range(moveSpeed * 0.5f, moveSpeed * 1.25f);
				}
			}
		}
	}
}
