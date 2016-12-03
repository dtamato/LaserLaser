using UnityEngine;
using System.Collections;
using  UnityEngine.UI;

public class TextShaker : MonoBehaviour
{
    [SerializeField] private float length = 0.05f;
    [SerializeField] private float speed = 0.25f;
    public int ID = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    gameObject.GetComponent<Text>().color = GameObject.Find("Player" + (ID+1)+" Overlay").GetComponent<SpriteRenderer>().color;
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, Mathf.PingPong(Time.time*speed, length),transform.rotation.w);
    }
}
