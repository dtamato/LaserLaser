using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
    [SerializeField] GameObject cannon;
    [SerializeField] Text scoreText;
    [SerializeField] private Text comboText;


    Rigidbody2D rb2d;
	Light light;

    public int score = 0;
    public int comboCount = 0;

	private int diamondCount = 0;

    private int crystalCount = 0;
    public int myPlayerID = 0;


    void Awake()
    {

        rb2d = this.GetComponentInChildren<Rigidbody2D>();
		light = this.GetComponentInChildren<Light> ();
        this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

	void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			comboText.text = "Combo: " + comboCount;
			//Debug.Log(comboCount);
		}
	}

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.transform.CompareTag("Boundary"))
        {

            rb2d.isKinematic = true;
            cannon.transform.position = other.contacts[0].point;
            cannon.transform.rotation = other.transform.rotation;
            this.transform.position = cannon.transform.position + 1.5f * cannon.transform.up;
            this.transform.GetComponent<SpriteRenderer>().enabled = false;
			cannon.GetComponentInChildren<Cannon> ().SetNewBaseAngle ();

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diamond"))
        {
            diamondCount++;
            //Debug.Log("I have a combo of: " + comboCount);
            score++;
            //Debug.Log(score);
			if (scoreText) { scoreText.text = "P" + (cannon.GetComponent<Cannon> ().GetPlayerID () + 1) + "- " + score.ToString ("00"); }


			StartCoroutine (PulsateLight ());

		// Enables the Paralysis script for a set period of time.
		if (other.tag == "Paralysis") {
			//Destroy (other.gameObject);
			//GetComponentInParent<Paralysis> ().enabled = true;
			//StartCoroutine (DisableScript ());
		}
    }


			Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();
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

    /*IEnumerator DisableScript()
	{
		yield return new WaitForSeconds (paralysisTimer);
		GetComponentInParent<Paralysis> ().enabled = false;
		GetComponentInParent<Paralysis> ().playerOne.GetComponent<CannonTester> ().enabled = true;
		GetComponentInParent<Paralysis> ().playerTwo.GetComponent<CannonTester> ().enabled = true;
		GetComponentInParent<Paralysis> ().playerThree.GetComponent<CannonTester> ().enabled = true;
		GetComponentInParent<Paralysis> ().playerFour.GetComponent<CannonTester> ().enabled = true;

	}*/
}




