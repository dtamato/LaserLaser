using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleParticle : MonoBehaviour {

    ParticleSystem ps;

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {

        float increase = 20 * Time.deltaTime;
        this.transform.localScale += new Vector3(increase, increase, increase);
        var mainPS = ps.main;
        Color reduceColor = new Color(0, 0, 0, 0.5f * Time.deltaTime);
        Color psColor = mainPS.startColor.color;
        Color newColor = psColor - reduceColor;
        mainPS.startColor = newColor;

        if(this.transform.localScale.x > 300)
        {
            Destroy(this.gameObject);
        }
	}

    public void ChangeColor (Color newColor)
    {
        ps = GetComponent<ParticleSystem>();
        var mainPS = ps.main;
        mainPS.startColor = newColor;
    }
}
