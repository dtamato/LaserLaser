using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GravityShifter : MonoBehaviour {

	[Header("Gravity Direction")]
	[SerializeField] float gravityStrength = 80;
	[SerializeField] float minSwitchTime = 5;
	[SerializeField] float maxSwitchTime = 10;
	bool switchingStarted = false;

	[Header("Gravity Image")]
	[SerializeField] GameObject gravityImagePrefab;
	GameObject gravityImageObject;
	Animator gravityImageAnimator;

	BaseGM gameManager;


	void Awake () {

		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
	}

	void Update () {

		if(gameManager.getState() == BaseGM.GAMESTATE.PREGAME && !switchingStarted) {

			switchingStarted = true;
			InitializeGravityShifter();
			StartCoroutine(ChangeGravityDirection());
		}
		else if(gameManager.getState() == BaseGM.GAMESTATE.POSTGAME) {

			Destroy(this.gameObject);
		}
	}

	void InitializeGravityShifter () {

		gravityImageObject = Instantiate(gravityImagePrefab, GameObject.Find("Canvas").transform) as GameObject;
		gravityImageObject.transform.SetSiblingIndex(0);
		gravityImageObject.GetComponent<RectTransform>().localScale = Vector3.one;
		gravityImageObject.GetComponent<Canvas>().overrideSorting = true;
		gravityImageObject.GetComponent<Canvas>().sortingLayerName = "Background";
		gravityImageObject.GetComponent<Canvas>().sortingOrder = 3;
		gravityImageAnimator = gravityImageObject.GetComponent<Animator>();
	}

	IEnumerator ChangeGravityDirection () {

		while (true) {

			Vector2 newGravityDirection = Vector2.zero;

			// Randomly get a new gravity direction
			do {

				float randomValue = Random.value;

				if(randomValue < 0.25f) {

					newGravityDirection = Vector2.left;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 90);
				}
				else if(randomValue < 0.5f) {

					newGravityDirection = Vector2.right;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 270);
				}
				else if(randomValue < 0.75f) {

					newGravityDirection = Vector2.up;
					gravityImageObject.transform.rotation = Quaternion.identity;
				}
				else {

					newGravityDirection = Vector2.down;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 180);
				}

			} while(gravityStrength * newGravityDirection == Physics2D.gravity);

			// Set new gravity direction
			Physics2D.gravity = gravityStrength * newGravityDirection;
			//Debug.Log("new direction: " + newGravityDirection);

			// Trigger animation for it
			gravityImageAnimator.SetTrigger("Show");
			gravityImageObject.GetComponent<RectTransform>().localPosition = Vector3.zero;

			// Wait before getting a new gravity direction
			yield return new WaitForSeconds(Random.Range(minSwitchTime, maxSwitchTime));
		}
	}

	void OnDestroy () {

		Physics2D.gravity = gravityStrength * Vector2.down;
	}
}
