using UnityEngine;
using System.Collections;

public class TBOverlay : MonoBehaviour
{
    private BaseGM gameManager;

    public int team;
    private Color teamColor;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        if (team == 1)
            teamColor = Color.red;
        else
            teamColor = Color.blue;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Only run when the laser is moved onto the trigger.
        if (other.gameObject.tag == "Player")
        {
            //Adjust the number of players on the team joined.
            if (team == 1)
                GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players++;
            else
                GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players++;

            //Reference the player and their ID.
            CannonCustomization player = other.gameObject.GetComponentInParent<CannonCustomization>();
            int pID = player.myID;

            //Adjust necessary player preferences.
            player.team = team;
            player.canChange = true;
            player.myTeamColor = teamColor;
            player.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = teamColor;

            //Pass change info to the GM.
            gameManager.setTeam(pID, team);
            gameManager.setTeamColour(pID, teamColor);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Only run once the laser is moved to a new location outside the collider.
        if (other.gameObject.tag == "Player")
        {
            //Adjust the number of players on the team joined.
            if (team == 1)
                GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team1Players--;
            else
                GameObject.Find("LobbyManager").GetComponent<LobbyManager>().team2Players--;

            //Reference the player and their ID.
            CannonCustomization player = other.gameObject.GetComponentInParent<CannonCustomization>();
            int pID = player.myID;

            //Adjust necessary player preferences.
            player.team = 0;
            player.canChange = false;
            player.myTeamColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            player.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);

            //Pass change info to the GM.
            gameManager.setTeam(pID, 0);
            gameManager.setTeamColour(pID, new Color(0.8f, 0.8f, 0.8f, 1f));
        }
    }
}
