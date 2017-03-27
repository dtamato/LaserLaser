using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

[DisallowMultipleComponent]
public class MainMenuController : MonoBehaviour {

	[Header("Rotation Parameters")]
	[SerializeField] float inputCooldown = 0.5f;
	[SerializeField] float rotationSpeed = 200f;
	int rotationDirection;
	float rotationAngle;
	float inputCooldownTimer;
	float currentAngle;
	float targetAngle;

	[Header("Game Mode Loading")]
	[SerializeField] GameObject[] gameModePrefabs;
	int gameModeIndex = 0;

	[Header("References")]
	[SerializeField] GameObject gameModeParent;
	[SerializeField] Image leftArrowImage;
	[SerializeField] Image rightArrowImage;
	Player rewiredPlayer;


	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer(0);
		rotationAngle = (360 / gameModeParent.transform.childCount);
		currentAngle = gameModeParent.transform.rotation.y;
		targetAngle = currentAngle;
		inputCooldownTimer = 0;
	}

	void Update () {

		ProcessInputs();
		RotateToTarget();
	}

	void ProcessInputs () {

		if(inputCooldownTimer <= 0 && targetAngle == currentAngle) {
			
			if(rewiredPlayer.GetAxisRaw("Horizontal") > 0) {

				targetAngle += rotationAngle;
				rotationDirection = 1;
				inputCooldownTimer = inputCooldown;

				gameModeIndex++;
				gameModeIndex %= gameModeParent.transform.childCount;

				leftArrowImage.color = Color.white;
				rightArrowImage.color = Color.yellow;
				StartCoroutine(ArrowBackToWhite());
			}
			else if(rewiredPlayer.GetAxisRaw("Horizontal") < 0) {

				targetAngle -= rotationAngle;
				rotationDirection = -1;
				inputCooldownTimer = inputCooldown;

				gameModeIndex--;
				gameModeIndex = (gameModeIndex < 0) ? gameModeParent.transform.childCount - 1 : gameModeIndex;

				leftArrowImage.color = Color.yellow;
				rightArrowImage.color = Color.white;
				StartCoroutine(ArrowBackToWhite());
			}
			else if(rewiredPlayer.GetButtonDown("Fire")) {

				Debug.Log("Loading game mode: " + gameModeIndex);

				if(gameModePrefabs.Length > 0) {

					//Instantiate(gameModePrefabs[gameModeIndex], Vector3.zero, Quaternion.identity);
				}

				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
		else if(inputCooldownTimer > 0) {

			inputCooldownTimer -= Time.deltaTime;
		}
	}

	void RotateToTarget () {

		targetAngle %= 360;
		float deltaAngle = (targetAngle - currentAngle) % 360;
		if(deltaAngle < 0) { deltaAngle += 360; }

		if(Mathf.Abs(deltaAngle) < 5f) {

			gameModeParent.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
			currentAngle = targetAngle;
			targetAngle %= 360;
		}
		else {
			
			gameModeParent.transform.Rotate(Vector3.up, rotationSpeed * rotationDirection * Time.deltaTime);
			//gameModeParent.transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, currentAngle, 0), Quaternion.Euler(0, Mathf.Sign(deltaAngle) * targetAngle, 0), rotationSpeed * Time.fixedDeltaTime);
			currentAngle = gameModeParent.GetComponent<RectTransform>().localRotation.eulerAngles.y;
		}
	}

	IEnumerator ArrowBackToWhite () {

		yield return new WaitForSeconds(0.25f);
		leftArrowImage.color = Color.white;
		rightArrowImage.color = Color.white;
	}
}
