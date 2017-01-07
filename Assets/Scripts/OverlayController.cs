using UnityEngine;
using System.Collections;

public class OverlayController : MonoBehaviour
{

    public int playerId;
    public GameObject playerCannon; // reference to the player's cannon
    public Vector2 resetPos;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCannon.GetComponent<CannonCustomization>().canChange = true;
            if (other.GetComponent<Laser>().myPlayerID == playerId)
            {
                other.GetComponent<Rigidbody2D>().transform.position = gameObject.transform.position; //setting the laser to the center of the field
                GameObject.Find("LobbyManager").GetComponent<LobbyManager>().readyPlayers++;
                //Debug.Log("Match! [ " + "My ID: " + playerId + ", Laser ID: " + other.GetComponent<Laser>().myPlayerID + "]");
            }
            else
            {
                other.GetComponent<Rigidbody2D>().AddForce(2200 * -this.transform.up); //bounce the player off the other's field
               // Debug.Log("Error, ID's do not match! [ " + "My ID: " + playerId + ", Laser ID: " + other.GetComponent<Laser>().myPlayerID + "]");
            }
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        playerCannon.GetComponent<CannonCustomization>().canChange = false; //make sure the player cannot change their colour unles they're in range of their field
        GameObject.Find("LobbyManager").GetComponent<LobbyManager>().readyPlayers--;
    }

}
