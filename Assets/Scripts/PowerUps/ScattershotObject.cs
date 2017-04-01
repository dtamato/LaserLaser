using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScattershotObject : MonoBehaviour
{
    BaseGM gameManager;
    [SerializeField] GameObject Laser;
    

    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        this.GetComponent<TrailRenderer>().sortingLayerName = this.GetComponent<SpriteRenderer>().sortingLayerName;
        this.GetComponent<TrailRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    void Start()
    {
        StartCoroutine(DestroyMe());
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0.3f, 0));
    }

    public void setLaser(GameObject laserReference)
    {
        Laser = laserReference;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Diamond")
        {
          //if (transform.parent.GetComponent<Laser>() != null)  | keeping only for testing different scenario purposes. Might not actually be useful in final build.
           //Debug.Log("Score: +1 ");
           Laser.GetComponent<Laser>().scoreCounter();

           // Create score canvas
           GameObject newScoreCounter = Instantiate(Laser.GetComponent<Laser>().scoreCounterPrefab, other.transform.position, Quaternion.identity) as GameObject;
            newScoreCounter.GetComponent<ScoreCounterCanvas>().SetText(Laser.GetComponent<Laser>().score);
           newScoreCounter.GetComponentInChildren<Text>().color = Laser.GetComponent<SpriteRenderer>().color;
        }

        if (other.gameObject.CompareTag("Boundary"))
        {

            Destroy(this.gameObject, 0.5f);
        }
    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
