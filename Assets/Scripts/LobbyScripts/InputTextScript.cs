using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InputTextScript : MonoBehaviour
{
    public int ID;
    public float displayTime = 2.0f;
    public bool check1, check2;

    void Start()
    {
        this.GetComponent<Text>().enabled = false;
        check1 = false;
        check2 = true;
    }

    void Update()
    {
        //gameObject.GetComponent<Text>().color = GameObject.Find("Player" + (ID + 1) + " Overlay").GetComponent<SpriteRenderer>().color;
    }
   
    //Adds time to the coroutine if the player is adjusting their settings many times in quick succession.
    public void checkText()
    {
        if (check1 == false)
        {
            check1 = true;
            StartCoroutine("showText");
        }
        else
            check2 = true;
    }

    IEnumerator showText()
    {
        Debug.Log("ran");
        this.GetComponent<Text>().enabled = true;
        while (check2)
        {
            check2 = false;
            yield return new WaitForSeconds(displayTime);
        }
        this.GetComponent<Text>().enabled = false;
        check1 = false;
        check2 = true;
    }
}
