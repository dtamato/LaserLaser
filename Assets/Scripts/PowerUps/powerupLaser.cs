using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This is the script used for the lasers being shot in the scattershot powerup.
public class powerupLaser : MonoBehaviour 
{
	//External References.
	BaseGM gameManager;
	[SerializeField] GameObject cannon;
	Light light;

	//Set Dynamically from GameManager.
	public Text scoreText;

	void Awake()
	{
		light = this.GetComponentInChildren<Light>();
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
		this.GetComponent<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
		this.GetComponent<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
	}

	void Start()
	{
		StartCoroutine (DestroyMe ());
	}

	void Update()
	{
		transform.Translate (new Vector3 (0, 0.25f, 0));
	}
	// Implement Score & Destroy this object.
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Diamond")
		{
			GetComponentInParent<Laser> ().scoreCounter ();
			//Debug.Log ("Works on powerup");
		}

		if (other.gameObject.tag == "PowerupBoundary") 
		{
			Destroy (this.gameObject);
			//Debug.Log ("Object destroyed");
		}

		Camera.main.GetComponent<CameraEffects>().ShakeCamera();
	}

	IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds (2);
		Destroy (this.gameObject);
	}
}
