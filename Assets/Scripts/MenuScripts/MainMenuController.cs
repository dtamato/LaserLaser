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
	bool loadingGame = false;

	[Header("Audio")]
	[SerializeField] AudioClip rotateAudio;
	[SerializeField] AudioClip confirmAudio;
	AudioSource audioSource;

	[Header("References")]
	[SerializeField] GameObject[] moviePlaneArray;
	[SerializeField] GameObject gameModeParent;
	[SerializeField] Image leftArrowImage;
	[SerializeField] Image rightArrowImage;
	[SerializeField] Image screenOverlay;
	[SerializeField] GameObject gravityShifterPrefab;
	Player rewiredPlayer;


	void Awake () {

		rewiredPlayer = ReInput.players.GetPlayer(0);
		audioSource = this.GetComponent<AudioSource>();
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

				audioSource.clip = rotateAudio;
				audioSource.Play();
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

				audioSource.clip = rotateAudio;
				audioSource.Play();
			}
			else if(rewiredPlayer.GetButtonDown("Fire") || rewiredPlayer.GetButtonDown("StartGame")) {

				if(gameModeIndex != 0) {

					Debug.Log("Loading game mode: " + gameModeIndex);
					for(int i = 0; i < moviePlaneArray.Length; i++) {

						moviePlaneArray[i].gameObject.SetActive(false);
					}

					audioSource.clip = confirmAudio;
					audioSource.Play();
					loadingGame = true;
				}
			}
		}
		else if(inputCooldownTimer > 0) {

			inputCooldownTimer -= Time.deltaTime;
		}

		if(loadingGame) {

			float newAlpha = screenOverlay.color.a + Time.deltaTime;
			screenOverlay.color = new Color(0, 0, 0, newAlpha);

			if(newAlpha >= 1) {

				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

				if(gameModePrefabs.Length > 0) {

					Instantiate(gameModePrefabs[gameModeIndex], Vector3.zero, Quaternion.identity);

					// Also instantiate gravity shifter prefab
					if(gameModeIndex == 2) {

						GameObject gravityShifter = Instantiate(gravityShifterPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						DontDestroyOnLoad(gravityShifter);
					}
				}
			}
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
