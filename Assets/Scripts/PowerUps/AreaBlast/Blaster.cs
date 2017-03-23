using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour 
{
	// Timer for the blaster.
	[SerializeField] float blasterTimer = 5;

	BaseGM gameManager;
	public int playerNum;
	public int teamNum;
	public int score;
	public string gameMode;
	private float transparent = 0.5f;

	void Awake()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
		gameMode = gameManager.gameMode;

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Diamonds"))
		{
			newScore();
		}
	}

	public void newScore()
	{
		// FFA scoring.
		if (gameMode == "FFA")
		{
			score++;
			gameManager.addScore(playerNum, score);
		}
		// Team scoring.
		else
		{
			gameManager.addToTeamScore (teamNum == 1);
		}
	}

	IEnumerator DistroyBlaster()
	{
		yield return new WaitForSeconds (blasterTimer);
		Destroy (this.gameObject);
	}
}
