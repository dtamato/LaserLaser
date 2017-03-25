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
	[SerializeField] GameObject stealTriggerPrefab;

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
		else if (other.transform.CompareTag ("Player")) {

			float randomX = other.transform.position.x + Random.Range(0, 2) * 2 - 1;
			float randomY = other.transform.position.y + Random.Range(0, 2) * 2 - 1;
			Vector3 randomPosition = new Vector3(randomX, randomY, 0);
			GameObject newCanvasObject = Instantiate(trickshotCanvasPrefab, randomPosition, Quaternion.identity) as GameObject;
			newCanvasObject.GetComponent<TrickshotCanvas>().SetText("#$%@");
			newCanvasObject.GetComponentInChildren<Text>().color = this.GetComponent<SpriteRenderer>().color;
			Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();
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

			// Create steal trigger
			GameObject newStealTrigger = Instantiate(stealTriggerPrefab, other.transform.position, Quaternion.identity) as GameObject;
			newStealTrigger.GetComponent<StealTrigger>().SetColor(this.transform.GetComponent<SpriteRenderer>().color);

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

		// Combos
		if(diamondCombo == 2) {

			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, trickshotCanvasPosition, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponent<TrickshotCanvas>().SetText("DOUBLE SHOT");
			newTrickshotCanvas.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;
		}
		else if(diamondCombo == 3) {

			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, trickshotCanvasPosition, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponent<TrickshotCanvas>().SetText("TRIPLE SHOT");
			newTrickshotCanvas.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;
		}
		else if(travelDistance > longShotDistance) {

			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, trickshotCanvasPosition, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponent<TrickshotCanvas>().SetText("LONG SHOT");
			newTrickshotCanvas.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;
		}

		// Trickshots
		if(bounceCombo > 0) {

			GameObject newTrickshotCanvas = Instantiate(trickshotCanvasPrefab, trickshotCanvasPosition, Quaternion.identity) as GameObject;
			newTrickshotCanvas.GetComponent<TrickshotCanvas>().SetText("TRICK SHOT");
			newTrickshotCanvas.GetComponentInChildren<Text>().color = this.GetComponentInChildren<SpriteRenderer>().color;
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
    #endregion
}