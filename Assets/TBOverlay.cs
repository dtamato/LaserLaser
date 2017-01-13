using UnityEngine;
using System.Collections;

public class TBOverlay : MonoBehaviour
{

    [SerializeField] private int team;

    private Color teamColour;
    public int teamValue;

    private GameObject[] playerCannons; // reference to the player's cannon
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    void Start()
    {
        if (team == 1)
            teamColour = Color.blue;
        else
            teamColour = Color.red;

        playerCannons = new GameObject[4];
        playerCannons[0] = player1;
        playerCannons[1] = player2;
        playerCannons[2] = player3;
        playerCannons[3] = player4;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (team == 1)
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players++;
        else
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players++;
        playerCannons[other.GetComponent<Laser>().myPlayerID].transform.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = teamColour;
        //other.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = teamColour;
        playerCannons[other.GetComponent<Laser>().myPlayerID].GetComponent<CannonCustomization>().team = team;
        playerCannons[other.GetComponent<Laser>().myPlayerID].GetComponent<CannonCustomization>().canChange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (team == 1)
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players--;
        else
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players--;
        playerCannons[other.GetComponent<Laser>().myPlayerID].transform.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
        playerCannons[other.GetComponent<Laser>().myPlayerID].GetComponent<CannonCustomization>().team = 0;
        playerCannons[other.GetComponent<Laser>().myPlayerID].GetComponent<CannonCustomization>().canChange = false;
    }
}
