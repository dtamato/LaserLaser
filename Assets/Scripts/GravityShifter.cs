using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GravityShifter : MonoBehaviour {

	[Header("Gravity Direction")]
	[SerializeField] float gravityStrength = 80;
	[SerializeField] float minSwitchTime = 5;
	[SerializeField] float maxSwitchTime = 10;
	Vector2 currentGravityDirection;
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

		if(gameManager.getState() == BaseGM.GAMESTATE.INGAME && !switchingStarted) {

			switchingStarted = true;
			InitializeGravityShifter();
			StartCoroutine(ChangeGravityDirection());
		}
		else if(gameManager.getState() == BaseGM.GAMESTATE.POSTGAME) {

			Destroy(this.gameObject);
		}
	}

	void InitializeGravityShifter () {

		currentGravityDirection = Physics2D.gravity;

		gravityImageObject = Instantiate(gravityImagePrefab, GameObject.Find("Canvas").transform) as GameObject;
		gravityImageObject.GetComponent<RectTransform>().localScale = Vector3.one;
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
					gravityImageObject.transform.rotation = Quaternion.identity;
				}
				else if(randomValue < 0.5f) {

					newGravityDirection = Vector2.right;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 180);
				}
				else if(randomValue < 0.75f) {

					newGravityDirection = Vector2.up;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 270);
				}
				else {

					newGravityDirection = Vector2.down;
					gravityImageObject.transform.rotation = Quaternion.Euler(0, 0, 90);
				}

			} while(newGravityDirection == gravityStrength * currentGravityDirection);

			// Set new gravity direction
			newGravityDirection *= gravityStrength;
			Physics2D.gravity = newGravityDirection;
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
