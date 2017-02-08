using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;

public class ColorList
{
    public bool isAvailable = true;
    public Color _color;

    public ColorList(bool avail, Color col)
    {
        isAvailable = avail; //controls whether or not the player can actually access the colour (making sure no two players can have the same colour)
        _color = col; //The colour the player can choose
    }
}

public class LobbyManager : MonoBehaviour {


    void FFAColourList() //The available colours for the FFA lobby
    {
		_colorlist[0] = new ColorList(false, new Color(1, 0, 0.4f)); // Thi's Magenta
		_colorlist[1] = new ColorList(false, new Color(0.21f, 1, 0.13f)); // Thi's Green
		_colorlist[2] = new ColorList(false, new Color(1, 0.89f, 0)); // Thi's Yellow
		_colorlist[3] = new ColorList(false, new Color(0.31f, 1, 0.87f)); // Thi's Blue
        _colorlist[4] = new ColorList(true, Color.magenta);
        _colorlist[5] = new ColorList(true, Color.cyan);
        _colorlist[6] = new ColorList(true, new Color(0.29f, 0.35f, 0.67f, 1f)); // Deep violet
        _colorlist[7] = new ColorList(true, new Color(0.95f, 0.62f, 0f, 1f)); // Orange
        _colorlist[8] = new ColorList(true, new Color(0f, 0.95f, 0.7f, 1f)); // Teal
        _colorlist[9] = new ColorList(true, new Color(0.65f, 0f, 0.73f, 1f)); // Purple
    }

    private BaseGM gameManager;
    private bool playerReady;
    private GameObject[] playerCannons; // reference to the player's cannon
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    public ColorList[] _colorlist = new ColorList[10];

    public int joinedPlayers;
    public int team1Players;
    public int team2Players;

    public string gameType;      //Passed from GM.
    

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        FFAColourList();
        playerCannons = new GameObject[4]; 
        playerCannons[0] = player1;
        playerCannons[1] = player2;
        playerCannons[2] = player3;
        playerCannons[3] = player4;

        for (int i = 0; i < playerCannons.Length; i++) //iterating through each player to set their colour to grey.
        {
            playerCannons[i].GetComponentInChildren<SpriteRenderer>().color = Color.gray;
            playerCannons[i].transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
            playerCannons[i].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
            playerCannons[i].transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,0f);
        }
        gameType = "FFA";
        SwitchTeamMode();
    }

    public int IncrementIndex(int idx) //if the player wants to change their colour forward ->
    {
        _colorlist[idx].isAvailable = true; //accessing the array of available colours contained within ColorManager
        do
        {
            idx++;
            idx %= 10;
        } while (!_colorlist[idx].isAvailable);
        return idx;
    }

    public int DecrementIndex(int idx) //if the player wants to change their colour backward <-
    {
        _colorlist[idx].isAvailable = true;
        do
        {
            idx = (idx - 1) % 10;
            idx = idx < 0 ? idx + 10 : idx; //is check 1 true? if yes, use check 2 (wraps around back to the end of the array when you're decrementing past the first element)
        } while (!_colorlist[idx].isAvailable);
        return idx;
    }

    public void UnjoinColour(int cIdx,int pId) //if the player leaves then return them to their position, disable their cannon (checked within PlayerActivationCheck()), and change their colour to grey
    {
        playerCannons[pId].GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        playerCannons[pId].transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
        playerCannons[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
        playerCannons[pId].GetComponentInChildren<Rigidbody2D>().isKinematic = false; // adding gravity so the player can fall into place 
        playerCannons[pId].GetComponentInChildren<Rigidbody2D>().transform.position = GameObject.Find("Player" + (pId + 1) + " Overlay").GetComponent<OverlayController>().resetPos; //resetting the player's cannon to its original position
        playerCannons[pId].transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0.8f,0.8f,0.8f,0f);
        GameObject.Find("JoinText" + pId).GetComponent<Text>().enabled = true; //the player's 'Press 'A' to join text'
    }

    public void UpdateColour(int cIdx, int pId) //if the player tries to change their colour, it will be updated here. Also, if the player joins too. 
    {
        playerCannons[pId].transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Laser colour
        playerCannons[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = _colorlist[cIdx]._color; //Trail renderer colour
        GameObject.Find("Player" + (pId + 1) + " Overlay").GetComponent<SpriteRenderer>().color = _colorlist[cIdx]._color; //Overlay colour
        playerCannons[pId].GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Cannon colour
        _colorlist[cIdx].isAvailable = false; //making sure other players cannot use the same colour
        playerCannons[pId].GetComponent<CannonCustomization>().myColor = _colorlist[cIdx]._color;   //Update Color variable, to be passed to the GM.
        playerCannons[pId].GetComponent<CannonCustomization>().inputText.GetComponent<Text>().color = _colorlist[cIdx]._color;
    }

    public void SwitchTeamMode()
    {
        if (gameType == "FFA") {    //If the game is in FFA Mode. Now changing to TB Mode.
            GameObject.Find("GameType").GetComponent<Text>().text = "Team Battle!";
            //Turn on the TB overlays / Team Zones.
            for (int x = 1; x < 3; x++) {
                GameObject teamOverlay = GameObject.Find("Team" + x);
                teamOverlay.GetComponent<SpriteRenderer>().enabled = true;
                teamOverlay.GetComponent<BoxCollider2D>().enabled = true;
            }
            //Turn off the FFA overlays.
            for (int i = 0; i < 4; i++) {
                GameObject ffaOverlay = GameObject.Find("Player" + (i + 1) + " Overlay");
                ffaOverlay.GetComponent<SpriteRenderer>().enabled = false;
                ffaOverlay.GetComponent<BoxCollider2D>().enabled = false;
                GameObject player = GameObject.Find("Player " + "(" + i + ")");
                player.GetComponent<CannonCustomization>().canChange = false;
                player.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f, 1f);     //???
            }

            gameType = "TB";
            gameManager.gameMode = "TB";
            for (int i = 0; i < 4; i++) {
                playerCannons[i].GetComponent<CannonCustomization>().team = 0;
                gameManager.setTeam(i, 0);
            }
        }

        else {                      //Else the game is in TB Mode. Now changing to FFA Mode.
            GameObject.Find("GameType").GetComponent<Text>().text = "Free For All!";
            //Turn off the TB overlays / Team Zones.
            for (int x = 1; x < 3; x++) {
                GameObject teamOverlay = GameObject.Find("Team" + x);
                teamOverlay.GetComponent<SpriteRenderer>().enabled = false;
                teamOverlay.GetComponent<BoxCollider2D>().enabled = false;
                team1Players = 0;
                team2Players = 0;
            }
            //Turn on the FFA overlays.
            for (int i = 0; i < 4; i++) {
                GameObject ffaOverlay = GameObject.Find("Player" + (i + 1) + " Overlay");
                ffaOverlay.GetComponent<SpriteRenderer>().enabled = true;
                ffaOverlay.GetComponent<BoxCollider2D>().enabled = true;
                GameObject player = GameObject.Find("Player " + "(" + i + ")");
                player.GetComponent<CannonCustomization>().team = 0;
                player.GetComponent<CannonCustomization>().canChange = false;
                player.transform.Find("ColourBand").GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f); //making the colour bands invisible
                player.GetComponent<CannonCustomization>().myTeamColor = new Color(0f,0f,0f,0f);
            }

            gameType = "FFA";
            gameManager.gameMode = "FFA";
            for (int i = 0; i < 4; i++) {
                playerCannons[i].GetComponent<CannonCustomization>().team = i;
                gameManager.setTeam(i, (i + 1));
            }
        }
    }

    //Called from CannonCustomization.cs. When player presses 'Start'.
    public void StartGameCheck()
    {
        //Requires 2 players to have joined the game.
        if (gameType == "FFA") {
            if (joinedPlayers >= 2)
            {
                Debug.Log("changing to FFA game");
                gameManager.changeScene(gameManager.mainGameSceneIndex);
            }
        }
        //Requires at least 1 player on each team.
        else if (gameType == "TB") {
            if (team1Players > 0 && team2Players > 0 && (team1Players + team2Players) == joinedPlayers)
            {
                Debug.Log("changing to TB game");
                gameManager.changeScene(gameManager.mainGameSceneIndex);
            }
        }
    }
}
