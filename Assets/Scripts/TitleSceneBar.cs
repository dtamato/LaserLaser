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


	void Start () {

		moveSpeed = Random.Range(moveSpeed * 0.5f, moveSpeed * 1.5f);
		colorBar.transform.localPosition = startPointTransform.localPosition;
	}

	void Update () {

		if(Time.time > startDelay && colorBar.transform.localPosition != endPointTransform.localPosition) {

			colorBar.transform.localPosition = Vector3.Lerp(colorBar.transform.localPosition, endPointTransform.localPosition, moveSpeed * Time.deltaTime);
		}
	}
}
