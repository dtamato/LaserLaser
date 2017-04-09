using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class SinglePlayerCannon : MonoBehaviour {

	//from customization script
	private int colorIdx; //the player's position within the color array
	public int sensitivity;

	//References.
	BaseGM gameManager;
	public Player rewiredPlayer;
	[SerializeField] SinglePlayerLaser pairedLaser;     // Permanent reference to ball, manually assigned in Lobby scene and in player prefab.
	[SerializeField] Rigidbody2D laserRB;   // Reference to the ball's RB.
	public bool inFlight;                   // True if laser/ball is in flight, false if stored in cannon.

	//Control values.
	[SerializeField] float baseRotationSpeed;
	[SerializeField] float maxBlastForce;
	[SerializeField] float maxAngleOffset;

	//Rotation
	[SerializeField] float currentRotationSpeed;
	int rotationModifier = 1;
	float minRotationSpeed = 1.5f;
	float maxRotationSpeed = 5.0f;

	private Transform LTransform;
	private Transform RTransform;
	private Transform MTransform;
	private Vector2 LPoint;
	private Vector2 RPoint;
	private Vector2 MPoint;
	private const float cornerOffset = 0.6f;
	private Vector2 MOrigin;
	private int Layer_Mask;
	private float axisMod;

	[SerializeField] private bool collidingLeft;
	[SerializeField] private bool collidingRight;

	//Angles
	float currentAngle;
	float baseAngle;
	float minAngle;
	float maxAngle;


	void Start()
	{
		laserRB = pairedLaser.GetComponent<Rigidbody2D>();
		rewiredPlayer = ReInput.players.GetPlayer(0);

		inFlight = false;
		sensitivity = 5;

		LTransform = transform.Find("LPoint").transform;
		RTransform = transform.Find("RPoint").transform;
		MTransform = transform.Find("MPoint").transform;
		LPoint = new Vector2(LTransform.position.x,LTransform.position.y);
		RPoint = new Vector2(RTransform.position.x, RTransform.position.y);
		MPoint = new Vector2(MTransform.position.x, MTransform.position.y);

		Layer_Mask = LayerMask.GetMask("Boundary");

		//Setup for rotation.
		if (maxAngleOffset < 0)
			maxAngleOffset *= -1;
		
		currentRotationSpeed = baseRotationSpeed;
		SetNewBaseAngle();
	}

	void Update()
	{
		if (!inFlight)
		{
			GetRotationInput();
			RestrictAngle();
			GetFireInput();
		}
	}

	#region Inputs
	void GetRotationInput()
	{
		axisMod = rewiredPlayer.GetAxis("Horizontal");
		LPoint = new Vector2(LTransform.position.x, LTransform.position.y); //making a Vector2 out of the object's transforms
		RPoint = new Vector2(RTransform.position.x, RTransform.position.y);
		MPoint = new Vector2(MTransform.position.x, MTransform.position.y);

		collidingLeft = Physics2D.Linecast(MPoint, LPoint, Layer_Mask);
		collidingRight = Physics2D.Linecast(MPoint ,RPoint, Layer_Mask);

		if (collidingLeft)
		{
			if (rotationModifier == -1)
			{
				axisMod = Mathf.Clamp(axisMod, -1f, 0f);
			}
			else
			{
				axisMod = Mathf.Clamp(axisMod, 0f, 1f);
			}
		}

		if (collidingRight)
		{
			if (rotationModifier == -1)
			{
				axisMod = Mathf.Clamp(axisMod, 0f, 1f);
			}
			else
			{
				axisMod = Mathf.Clamp(axisMod, -1f, 0f);
			}
		}

		currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, minRotationSpeed, maxRotationSpeed);
		this.transform.Rotate(currentRotationSpeed * rotationModifier * Vector3.forward * -axisMod);
	}
		
	void RestrictAngle()
	{
		currentAngle = this.transform.rotation.eulerAngles.z;
		if (currentAngle < 0) { currentAngle += 360; }
		if (currentAngle >= maxAngle && currentAngle <= maxAngle + 5)
		{
			this.transform.rotation = Quaternion.Euler(0, 0, maxAngle);
		}
		else if (currentAngle <= minAngle && currentAngle >= minAngle - 5)
		{
			this.transform.rotation = Quaternion.Euler(0, 0, minAngle);
		}
	}
		
	void GetFireInput()
	{
		if (rewiredPlayer.GetButtonDown("Fire"))
		{
			FireLaser(maxBlastForce);	
		}
	}

	void FireLaser (float blastForce) {

		laserRB.bodyType = RigidbodyType2D.Dynamic;
		laserRB.GetComponent<Collider2D>().isTrigger = false;
		laserRB.AddForce(blastForce * this.transform.up);
		pairedLaser.transform.GetComponent<SpriteRenderer>().enabled = true;
		pairedLaser.transform.GetComponent<TrailRenderer>().enabled = true;
		inFlight = true;
		this.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.15f);
		this.GetComponent<AudioSource>().Play();
	}
	#endregion

	#region Collision
	void OnTriggerEnter2D(Collider2D other) {

		if(other.CompareTag("Spike")) {

			pairedLaser.StartCoroutine("HitSpike");
			FireLaser(0);
		}
	}
	#endregion

	#region Setters

	// Called from Laser.cs
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

	//Called from CannonCustomization.cs in lobby when sensitivity is changed.
	public void ChangeRotationSpeed(float increment)
	{
		baseRotationSpeed += increment;
		baseRotationSpeed = Mathf.Clamp(baseRotationSpeed, minRotationSpeed, maxRotationSpeed);
	}

	//Called from BaseGM.cs in main game when player is instantiated.
	public void ApplyRotationSpeed(int sensitivity)
	{
		baseRotationSpeed = 1.0f + (sensitivity * 0.5f);
		currentRotationSpeed = baseRotationSpeed;
	}

	//Called from Slow.cs.
	public void ModifyRotationSpeed(float newSpeed)
	{
		baseRotationSpeed = newSpeed;
	}

	public void SetIsPaused(bool isPaused)
	{
		gameManager.SetPaused(isPaused);
	}

	#endregion

	#region Getters
	public float GetBaseSpeed()
	{
		return baseRotationSpeed;
	}

	public GameObject GetLaser ()
	{
		return pairedLaser.gameObject;
	}

	public Player GetRewiredPlayer()
	{
		return rewiredPlayer;
	}
	#endregion
}
