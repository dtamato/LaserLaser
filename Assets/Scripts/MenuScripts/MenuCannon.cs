using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class MenuCannon : MonoBehaviour {

	[Header("References")]
	[SerializeField] int playerId;
	[SerializeField] MenuLaser pairedLaser;     // Permanent reference to ball, manually assigned in Lobby scene and in player prefab.
	[SerializeField] GameObject cannonPivot;
	[SerializeField] GameObject cannonExtension;
	[SerializeField] GameObject aimUI;
	[SerializeField] GameObject shootUI;
	[SerializeField] GameObject leftDiamondArrows;
	[SerializeField] GameObject rightDiamondArrows;

	[Header("Parameters")]
	[SerializeField] float baseRotationSpeed;
	[SerializeField] float maxBlastForce;
	[SerializeField] float maxAngleOffset;

	// Rotation
	Player rewiredPlayer;
	Rigidbody2D laserRB;
	float currentRotationSpeed;
	int rotationModifier = 1;
	float minRotationSpeed = 1.5f;
	float maxRotationSpeed = 5.0f;
	bool inFlight;

	//Angles
	float currentAngle;
	float baseAngle;
	float minAngle;
	float maxAngle;


	void Awake()
	{
		laserRB = pairedLaser.GetComponent<Rigidbody2D>();

		rewiredPlayer = ReInput.players.GetPlayer(playerId);

		if (maxAngleOffset < 0)
			maxAngleOffset *= -1;

		currentRotationSpeed = baseRotationSpeed;
		SetNewBaseAngle();

		shootUI.SetActive (false);
	}

	void Update()
	{
		if (!inFlight) {
			
			GetRotationInput();
			RestrictAngle();
			CheckFireInput();
		}
	}

	void GetRotationInput()
	{
		// Get controller joystick input
		if (rewiredPlayer.GetAxisRaw("Horizontal") < 0)
		{
			currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			cannonPivot.transform.Rotate(currentRotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxisRaw("Horizontal") > 0)
		{
			currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
			cannonPivot.transform.Rotate(-currentRotationSpeed * rotationModifier * Vector3.forward);
		}
		else if (rewiredPlayer.GetAxisRaw("Horizontal") == 0)
		{
			currentRotationSpeed = baseRotationSpeed;
		}

		// Check for collisions from parallel walls
		// rb2d.isTouchingLayers() ???

		if (rewiredPlayer.GetAxisRaw ("Horizontal") != 0 && aimUI.activeSelf) {

			aimUI.SetActive (false);
			shootUI.SetActive (true);
		}
	}

	void RestrictAngle()
	{
//		currentAngle = cannonPivot.transform.rotation.eulerAngles.z;
//		if (currentAngle < 0) { currentAngle += 360; }
//		if (currentAngle >= maxAngle && currentAngle <= maxAngle + 5)
//		{
//			cannonPivot.transform.rotation = Quaternion.Euler(0, 0, maxAngle);
//		}
//		else if (currentAngle <= minAngle && currentAngle >= minAngle - 5)
//		{
//			cannonPivot.transform.rotation = Quaternion.Euler(0, 0, minAngle);
//		}
	}

	void CheckFireInput()
	{
		if (rewiredPlayer.GetButtonDown("Fire"))
		{
			laserRB.bodyType = RigidbodyType2D.Dynamic;
			laserRB.AddForce(maxBlastForce * cannonExtension.transform.up);
			pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
			pairedLaser.transform.GetComponent<TrailRenderer>().enabled = true;
			this.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.5f);
			this.GetComponent<AudioSource>().Play();
			inFlight = true;

			if(shootUI.activeSelf) { 

				shootUI.SetActive (false);
				leftDiamondArrows.SetActive(true);
				rightDiamondArrows.SetActive(true);
			}
		}
	}
		
	public void SetNewBaseAngle()
	{
		baseAngle = this.transform.rotation.eulerAngles.z;

		if (baseAngle < 0) { baseAngle %= 360; }
		minAngle = baseAngle - maxAngleOffset;

		if (minAngle < 0) { minAngle += 360; }
		maxAngle = baseAngle + maxAngleOffset;

		if (maxAngle < 0) { maxAngle += 360; }

		// Change rotation modifier if upside down
		rotationModifier = (this.transform.up == Vector3.down) ? -1 : 1;
	}

	public void SetInFlight (bool isInFlight) {

		inFlight = isInFlight;
	}

	public Player GetRewiredPlayer () {

		return rewiredPlayer;
	}
}
