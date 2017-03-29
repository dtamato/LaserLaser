using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScattershotObject : MonoBehaviour
{
    BaseGM gameManager;
    [SerializeField] GameObject cannon;

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
        transform.Translate(new Vector3(0, 0.50f, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Diamond")
        {
            if (other.GetComponentInParent<Laser>() != null)
            {
                GetComponentInParent<Laser>().scoreCounter();
            }
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
