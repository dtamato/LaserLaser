using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;

public class LobbyManager : MonoBehaviour {

    
    [SerializeField] int playerId = 0;

    public Color mycolor;
    private bool playerReady;
    private bool canChange = false;
    private int colorIdx;
    private Player _rewiredPlayer;
    private bool hasJoined = false;
    public Vector3 resetPos;


    void Awake()
    {
        _rewiredPlayer = ReInput.players.GetPlayer(playerId);
    }

    // Use this for initialization
    private void Start()
    {
        colorIdx = playerId;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canChange = false; //make sure the player cannot change their colour unles they're in range of their field
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canChange = true;
            if (other.GetComponent<Laser>().myPlayerID == playerId)
            {
                other.gameObject.GetComponent<Rigidbody2D>().transform.position = gameObject.transform.position; //setting the laser to the center of the field
            }
            else
            {
                    other.GetComponent<Rigidbody2D>().AddForce(2200 * -this.transform.up); //bounce the player off the other's field
            }

        }
        
    }

    void ProcessInputs()
    {
        if (_rewiredPlayer.GetButtonDown("RButt") && canChange) //can only change if the player is within the field
        {
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = true; //accessing the array of available colours contained within ColorManager
            do
            {
                colorIdx++;
                colorIdx %= 10;
                //Debug.Log(colorIdx);
            } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable);

        }

        if (_rewiredPlayer.GetButtonDown("LButt") && canChange)
        {
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = true;
            do
            {

                colorIdx = (colorIdx - 1) % 10;
                colorIdx = colorIdx < 0 ? colorIdx + 10 : colorIdx; //is check 1 true? if yes, use check 2 (wraps around back to the end of the array when you're decrementing past the first element)

                //Debug.Log(colorIdx);
            } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable);

        }

        if (_rewiredPlayer.GetButtonDown("Fire") && !hasJoined)
        {
            hasJoined = true;
            GameObject.Find("JoinText" + playerId).GetComponent<Text>().enabled = false;

        }
        else if (_rewiredPlayer.GetButtonDown("Back") && hasJoined)
        {
            hasJoined = false;
            GameObject.Find("Laser" + playerId).GetComponent<SpriteRenderer>().color = Color.gray; //graying out the player to show that it is inactive
            GameObject.Find("Laser" + playerId).GetComponent<Rigidbody2D>().isKinematic = false; // adding gravity so the player can fall into place
            GameObject.Find("Laser" + playerId).GetComponent<Rigidbody2D>().transform.position = resetPos; //resetting the player's cannon to its original position
            GameObject.Find("JoinText" + playerId).GetComponent<Text>().enabled = true; //the player's 'Press 'A' to join text'

        }

        if (_rewiredPlayer.GetButtonDown("Setting"))
        {
            Debug.Log("Opening Settings...");
        }
            

    }

    void PlayerActivationCheck()
    {
        if (hasJoined)
            GameObject.Find("Cannon" + playerId).GetComponent<Cannon>().enabled = true; //if the player has joined, allow them to control the cannon, else disable it  
        else
            GameObject.Find("Cannon" + playerId).GetComponent<Cannon>().enabled = false;
    }

    void UpdateColor()
    {
        if (GameObject.Find("Laser" + playerId) != null && hasJoined) //if we can find the laser within the scene, and it hasn't been destroyed
        {
            GameObject.Find("Laser" + playerId).GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx]._color; //going to the list of available colours to set the player's colour correctly
            gameObject.GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx]._color;
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = false; //making sure other players cannot use the same colour
        }
        if (GameObject.Find("Laser" + playerId) != null && !hasJoined) //if the player leaves, then return them to their position, disable their cannon (checked within PlayerActivationCheck()), and change their colour to grey
        {
            GameObject.Find("Cannon Sprite" + playerId).GetComponent<SpriteRenderer>().color = Color.gray;
            gameObject.GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx]._color;
            GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[colorIdx].isAvailable = false;
        } 
    }

    // Update is called once per frame
	void Update ()
    {
        ProcessInputs();
        PlayerActivationCheck();
        UpdateColor();

    }



}
