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
                other.GetComponentInChildren<Rigidbody2D>().transform.position = gameObject.transform.position; //setting the laser to the center of the field
            }
            else
            {
                other.GetComponentInChildren<Rigidbody2D>().AddForce(2200 * -this.transform.up); //bounce the player off the other's field
            }
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        playerCannon.GetComponent<CannonCustomization>().canChange = false; //make sure the player cannot change their colour unles they're in range of their field
    }

}
