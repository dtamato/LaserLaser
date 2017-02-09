using UnityEngine;
using System.Collections;

public class TBOverlay : MonoBehaviour
{
    private BaseGM gameManager;

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
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();

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
        //Adjust the number of players on the team joined.
        if (team == 1)
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players++;
        else
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players++;
        if (!other.gameObject.GetComponent<CannonCustomization>()) return;
        int pID = other.gameObject.GetComponentInParent<CannonCustomization>().myID;
        playerCannons[pID].transform.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = teamColour;

        CannonCustomization player = playerCannons[pID].GetComponent<CannonCustomization>();
        player.team = team;
        player.canChange = true;
        player.myTeamColor = teamColour;

        //Send team info to the GM.
        gameManager.setTeam(pID, team);
        gameManager.setTeamColour(pID, teamColour);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Adjust the number of players on the team joined.
        if (team == 1)
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players--;
        else
            GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players--;
        if (!other.gameObject.GetComponent<Laser>()) return;
        int pID = other.GetComponent<Laser>().myPlayerID;
        playerCannons[pID].transform.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);

        CannonCustomization player = playerCannons[pID].GetComponent<CannonCustomization>();
        player.myTeamColor = new Color(0.8f,0.8f,0.8f,1f);
        player.team = 0;
        player.canChange = false;

        //Send team info to the GM.
        gameManager.setTeam(pID, 0);
        gameManager.setTeamColour(pID, new Color(0.8f, 0.8f, 0.8f, 1f));
    }
}
