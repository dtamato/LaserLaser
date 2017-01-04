using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;

public class LobbyManager : MonoBehaviour {

    
    [SerializeField] int playerId = 0;

    public Color mycolor;
    private bool playerReady;
    public Vector3 resetPos;

    public GameObject playerCannon; // reference to the player's cannon


    void Start()
    {
        playerCannon.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        playerCannon.transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
        playerCannon.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        playerCannon.GetComponent<Cannon>().canChange = false; //make sure the player cannot change their colour unles they're in range of their field
        //Debug.Log("Can change = " + playerCannon.GetComponent<Cannon>().canChange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCannon.GetComponent<Cannon>().canChange = true;
            //Debug.Log("Can change = " + playerCannon.GetComponent<Cannon>().canChange);
            if (other.GetComponent<Laser>().myPlayerID == playerId)
            {
                other.GetComponentInChildren<Rigidbody2D>().transform.position = gameObject.transform.position; //setting the laser to the center of the field
            }
            else
            {
                    other.GetComponentInChildren<Rigidbody2D>().AddForce(2200 * -this.transform.up); //bounce the player off the other's field
            }
        }
    }

    public void UnjoinColour() //if the player leaves, then return them to their position, disable their cannon (checked within PlayerActivationCheck()), and change their colour to grey
    {
        playerCannon.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        playerCannon.transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
        playerCannon.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
        playerCannon.GetComponentInChildren<Rigidbody2D>().isKinematic = false; // adding gravity so the player can fall into place 
        playerCannon.GetComponentInChildren<Rigidbody2D>().transform.position = resetPos; //resetting the player's cannon to its original position
        GameObject.Find("JoinText" + playerId).GetComponent<Text>().enabled = true; //the player's 'Press 'A' to join text'
        //GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx].isAvailable = true;
    }

    public void UpdateColour() //if we can find the laser within the scene, and it hasn't been destroyed
    {
        //Debug.Log("Trying to change colour...");
        playerCannon.transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color; //going through the list of available colours to set the player's colour correctly
        playerCannon.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        gameObject.GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        playerCannon.GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx].isAvailable = false; //making sure other players cannot use the same colour
    }

    public void JoinColour()
    {
        //Debug.Log("Trying to change colour...");
        playerCannon.transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color; //going through the list of available colours to set the player's colour correctly
        playerCannon.transform.Find("Laser").GetComponent<TrailRenderer>().material.color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        gameObject.GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        playerCannon.GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx]._color;
        GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx].isAvailable = false; //making sure other players cannot use the same colour
    }
    


}
