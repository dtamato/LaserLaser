using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    // References
    BaseGM gameManager;
    GameObject cannon;
    Rigidbody2D rb2d;
    Player rewiredPlayer;

    //Score and other metrics
    public int score = 0;
    public int myTeam;
    private bool sendResults;
    private string gameMode;
	private int diamondCombo = 0;
	private int bounceCombo = 0;
	private int controlCombo = 0;
	private float pitchTracker = 1;
	private Vector3 shotStartPosition = Vector3.zero;

	[SerializeField] GameObject scoreCounterPrefab;
	[SerializeField] GameObject comboCounterPrefab;
	[SerializeField] GameObject trickshotCanvasPrefab;
	[SerializeField] Color comboTextColor;

	[Header("Steal Trigger")]
	[SerializeField] GameObject stealTriggerPrefab;
	float playerCollisionCooldown = 2;
	float playerCollisionTimer = 0;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
		cannon = this.transform.parent.gameObject;
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
        this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;

        //Record the game mode, for scoring purposes.
        gameMode = gameManager.gameMode;
    }

	void Update () {

		UpdateTimers();
	}

	void UpdateTimers () {

		if(playerCollisionTimer > 0) { playerCollisionTimer -= Time.deltaTime; }
	}

    void OnCollisionEnter2D(Collision2D other)
    {
		if (other.transform.CompareTag ("Boundary")) {
			
			rb2d.bodyType = RigidbodyType2D.Static;
			rb2d.GetComponent<Collider2D>().isTrigger = true;
			cannon.transform.position = other.contacts [0].point;
			cannon.transform.rotation = other.transform.rotation;
			this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
			this.transform.GetComponent<SpriteRenderer> ().enabled = false;
			this.transform.GetComponent<TrailRenderer> ().enabled = false;
			cannon.GetComponentInChildren<Cannon> ().SetNewBaseAngle ();
			//cannon.GetComponentInChildren<Cannon>().SetStoredLaser(this);
			cannon.GetComponent<Cannon> ().inFlight = false;
			shotStartPosition = this.transform.position;

			// Show final combo count
			if(controlCombo > 1) {

				//float randomX = this.transform.position.x + Random.Range(0, 2) * 2 - 1;
				float randomX = this.transform.position.x;
				float randomY = this.transform.position.y + Random.Range(0, 2) * 2 - 1;
				Vector3 comboCounterPosition = new Vector3(randomX, randomY, 0);
				GameObject newComboCounter = Instantiate(comboCounterPrefab, comboCounterPosition, Quaternion.identity) as GameObject;
				newComboCounter.GetComponent<ComboCounterCanvas>().SetText(controlCombo);
				newComboCounter.GetComponentInChildren<Text>().color = Color.white;
			}

			diamondCombo = 0;
			bounceCombo = 0;
			controlCombo = 0;
			pitchTracker = 1;
		}
		else if (other.transform.CompareTag ("Player") && playerCollisionTimer <= 0) {

			float randomX = other.transform.position.x + Random.Range(0, 2) * 2 - 1;
			float randomY = other.transform.position.y + Random.Range(0, 2) * 2 - 1;
			Vector3 randomPosition = new Vector3(randomX, randomY, 0);
			GameObject newCanvasObject = Instantiate(trickshotCanvasPrefab, randomPosition, Quaternion.identity) as GameObject;
			newCanvasObject.GetComponentInChildren<Text>().fontSize = 80;
			string textToShow = Random.value < 0.5f ? "*@$&" : "#$%@";
			newCanvasObject.GetComponent<TrickshotCanvas>().SetText(textToShow);

			Color textColor = this.GetComponent<SpriteRenderer>().color;
			textColor = new Color(textColor.r, textColor.g, textColor.b, 0.7f);
			newCanvasObject.GetComponentInChildren<Text>().color = textColor;

			this.GetComponent<AudioSource>().Play();
			Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();

			playerCollisionTimer = playerCollisionCooldown;
		}
		else if(other.transform.CompareTag("Bouncer")) {

			bounceCombo++;
		}
		else if(other.transform.CompareTag("ControlBouncer")) {

			controlCombo++;

			pitchTracker += 0.05f;
			this.GetComponent<AudioSource>().pitch = pitchTracker;
			this.GetComponent<AudioSource>().Play();

			float randomX = other.transform.position.x + Random.Range(0, 2) * 2 - 1;
			float randomY = other.transform.position.y + Random.Range(0, 2) * 2 - 1;
			Vector3 comboCounterPosition = new Vector3(randomX, randomY, 0);
			GameObject newComboCounter = Instantiate(comboCounterPrefab, comboCounterPosition, Quaternion.identity) as GameObject;
			newComboCounter.GetComponent<ComboCounterCanvas>().SetText(controlCombo);
			newComboCounter.GetComponentInChildren<Text>().color = comboTextColor;
			StartCoroutine(Rumble(0.1f));
		}
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("Diamond"))
		{
			scoreCounter ();

			// Create score canvas
			GameObject newScoreCounter = Instantiate(scoreCounterPrefab, other.transform.position, Quaternion.identity) as GameObject;
			newScoreCounter.GetComponent<ScoreCounterCanvas>().SetText(score);
			newScoreCounter.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;

			// Trickshots
			diamondCombo++;
			CheckIfTrickshot(other.transform.position);
		}
    }

	void CheckIfTrickshot (Vector3 diamondPosition) {

		float randomX = diamondPosition.x + Random.Range(0, 2) * 2 - 1;
		float yPos = diamondPosition.y - 1;
		Vector3 trickshotCanvasPosition = new Vector3(randomX, yPos, 0);
		float travelDistance = Vector3.Distance(diamondPosition, shotStartPosition);
		const float longShotDistance = 10f;
		string canvasText = "";

		// Combos
		if(diamondCombo == 2) {

			canvasText += "DOUBLE ";
			gameManager.doubleshots[cannon.GetComponent<Cannon>().playerId]++;
		}
		else if(diamondCombo == 3) {

			canvasText += "TRIPLE ";
			gameManager.tripleshots[cannon.GetComponent<Cannon>().playerId]++;
		}

		if(travelDistance > longShotDistance) {

			canvasText += "LONG ";
			gameManager.longshots[cannon.GetComponent<Cannon>().playerId]++;
		}

		// Trickshots
		if(bounceCombo > 0) {

			canvasText += "TRICK ";
			gameManager.trickshots[cannon.GetComponent<Cannon>().playerId]++;
		}

		if(canvasText != "") {

			canvasText += (Random.value < 0.65f) ? "SHOT" : (Random.value < 0.5f ? "SHOT!?" : "SHOT!");
			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, trickshotCanvasPosition, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponent<TrickshotCanvas>().SetText(canvasText);
			newTrickshotCanvas.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;

			// Clamp x position of canvas
			float minX = -6;
			float maxX = 6;
			float newX = Mathf.Clamp(newTrickshotCanvas.GetComponent<RectTransform>().position.x, minX, maxX);
			Vector3 canvasPosition = newTrickshotCanvas.GetComponent<RectTransform>().position;
			newTrickshotCanvas.GetComponent<RectTransform>().position = new Vector3(newX, canvasPosition.y, canvasPosition.z);
		}
		else {

			// Only create steal trigger if there was no bonus shot
			GameObject newStealTrigger = Instantiate(stealTriggerPrefab, diamondPosition, Quaternion.identity) as GameObject;
			newStealTrigger.GetComponent<StealTrigger>().SetColor(this.transform.GetComponent<SpriteRenderer>().color);
		}
	}

	public void scoreCounter()
	{
		//FFA scoring.
		if (gameMode == "FFA")
		{
			score++;
			gameManager.addScore(cannon.GetComponent<Cannon>().GetPlayerID(), score);//this is where score text is set
			//Debug.Log("works on regular");
		}
		//Team scoring.
		else
		{
			gameManager.addToTeamScore (myTeam == 1);
			//Debug.Log("works on regular2");
		}
	}

    public void ChangeColor(Color newColor)
    {
        this.GetComponent<SpriteRenderer>().color = newColor;
    }

    #region Rumble
    // Variable length full-intensity rumble function
    public IEnumerator Rumble(float duration)
    {
        rewiredPlayer = cannon.GetComponentInChildren<Cannon>().GetRewiredPlayer();

        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(1.0f, 1.0f);
        }
        yield return new WaitForSeconds(duration);
        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }

    // Variable length low-intensity bump function
    public IEnumerator Bump(float duration)
    {
        rewiredPlayer = cannon.GetComponentInChildren<Cannon>().GetRewiredPlayer();

        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(0.25f, 0.25f);
        }
        yield return new WaitForSeconds(duration);
        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }

    // Variable direction half-second rumble function 
    public IEnumerator DirectionalRumble(float leftIntensity, float rightIntensity)
    {
        rewiredPlayer = cannon.GetComponentInChildren<Cannon>().GetRewiredPlayer();

        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(leftIntensity, rightIntensity);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Joystick j in rewiredPlayer.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }
    #endregion

    #region Setters

    public void setGameMode(string newGM)
    {
        gameMode = newGM;
    }
    #endregion

    #region Getters

    public string GetGameMode()
    {
        return gameMode;
    }

	public Color getColor()
	{
		return this.GetComponent<SpriteRenderer> ().color;
	}

    public GameObject GetScoreCounter()
    {
        return scoreCounterPrefab;
    }
    #endregion
}