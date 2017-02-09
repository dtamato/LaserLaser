using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    //External References.
    BaseGM gameManager;
    [SerializeField] GameObject cannon;
    Rigidbody2D rb2d;
    Light light;

    //Set Dynamically from GameManager.
    public Text scoreText;
    public Text comboText;

    //Score and other metrics.
    public int score = 0;
    public int comboCount = 0;
    private int diamondCount = 0;
    public int myPlayerID;
    public int myTeam;
    private bool sendResults;
    private string gameMode;

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
        light = this.GetComponentInChildren<Light>();
        this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;

        //Record the game mode, for scoring purposes.
        gameMode = gameManager.gameMode;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (gameMode == "FFA")
            {
                scoreText.text = "P" + (myPlayerID + 1) + ": " + score;
                comboText.text = "Combo: " + comboCount;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Boundary"))
        {
            //rb2d.isKinematic = true;
            rb2d.bodyType = RigidbodyType2D.Static; // HERE
            cannon.transform.position = other.contacts[0].point;
            cannon.transform.rotation = other.transform.rotation;
            this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
            cannon.GetComponentInChildren<Cannon>().SetNewBaseAngle();
            cannon.GetComponentInChildren<Cannon>().SetStoredLaser(this);

            //Only implement combos in FFA.
            if (gameMode == "FFA")
            {
                if (diamondCount == 0)
                {
                    score += comboCount;
                    comboCount = 0;
                }
                else
                {
                    comboCount++;
                    diamondCount = 0;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diamond"))
        {
            diamondCount++;     //For combo tracking.

            //FFA scoring.
            if (gameMode == "FFA")
            {
                score++;
                gameManager.addScore(myPlayerID, score);
            }
            //Team scoring.
            else
            {
				gameManager.addToTeamScore (myTeam == 1);
            }
        }

        Camera.main.GetComponent<CameraEffects>().ShakeCamera();
    }

    IEnumerator PulsateLight()
    {
        // Save current light settings and create temp variables
        float initialLightRange = light.range;
        float initialLightIntensity = light.intensity;
        float maxLightRangeSize = 2f;
        float growSpeed = 50;
        float waitTime = 0.25f;
        // Intensify light
        while (light.range < maxLightRangeSize * initialLightRange)
        {
            light.range += growSpeed * Time.deltaTime;
            light.intensity += growSpeed * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        // Return light to initial settings
        while (light.range > initialLightRange)
        {
            light.range -= growSpeed * Time.deltaTime;
            light.intensity -= growSpeed * Time.deltaTime;
            yield return null;
        }
        light.range = initialLightRange;
        light.intensity = initialLightIntensity;
    }

    public void ChangeColor(Color newColor)
    {
        this.GetComponent<SpriteRenderer>().color = newColor;
    }
}