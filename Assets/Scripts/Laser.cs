using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{
	[SerializeField] float paralysisTimer = 2;
    [SerializeField] GameObject cannon;
    [SerializeField] Text scoreText;
    [SerializeField] private Text comboText;


    Rigidbody2D rb2d;

    public int score = 0;
    public int comboCount = 0;
    private int crystalCount = 0;
    public int myPlayerID = 0;

    void Awake()
    {

        rb2d = this.GetComponentInChildren<Rigidbody2D>();
        this.GetComponentInChildren<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        this.GetComponentInChildren<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
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
            
			if (cannon.GetComponentInChildren<Cannon> ()) {
				
				cannon.GetComponentInChildren<Cannon> ().SetNewBaseAngle ();
			}
			else if (cannon.GetComponentInChildren<CannonTester> ()) {

				cannon.GetComponentInChildren<CannonTester> ().SetNewBaseAngle ();
			}

            if (crystalCount == 0)
            {
                score += comboCount;
                comboCount = 0;
            }
            else
            {
                comboCount++;
                crystalCount = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        //Destroy(this.gameObject);
        if (other.CompareTag("Item"))
        {
            crystalCount++;
            //Debug.Log("I have a combo of: " + comboCount);
            score++;
            //Debug.Log(score);
			if (cannon.GetComponent<Cannon> ()) {
			
				scoreText.text = "P" + (cannon.GetComponent<Cannon> ().GetPlayerID () + 1) + "- " + score.ToString ("00");
			}
			else if (cannon.GetComponent<CannonTester> ()) {

				scoreText.text = "P" + (cannon.GetComponent<CannonTester> ().GetPlayerID () + 1) + "- " + score.ToString ("00");
			}
			//Camera.main.GetComponent<CameraEffects> ().ShakeCamera ();
        }

		// Enables the Paralysis script for a set period of time.
		if (other.tag == "Paralysis") {
			//Destroy (other.gameObject);
			//GetComponentInParent<Paralysis> ().enabled = true;
			//StartCoroutine (DisableScript ());
		}
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            comboText.text = "Combo: " + comboCount;
            //Debug.Log(comboCount);
        }
            
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
