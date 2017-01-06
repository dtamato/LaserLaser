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
        _colorlist[0] = new ColorList(false, Color.red);
        _colorlist[1] = new ColorList(false, Color.blue);
        _colorlist[2] = new ColorList(false, Color.green);
        _colorlist[3] = new ColorList(false, Color.magenta);
        _colorlist[4] = new ColorList(true, Color.yellow); //need to change due to it being too close to the crystal's colour
        _colorlist[5] = new ColorList(true, Color.cyan);
        _colorlist[6] = new ColorList(true, new Color(0.29f, 0.35f, 0.67f, 1f));
        _colorlist[7] = new ColorList(true, new Color(0.95f, 0.62f, 0f, 1f));
        _colorlist[8] = new ColorList(true, new Color(0f, 0.95f, 0.7f, 1f));
        _colorlist[9] = new ColorList(true, new Color(0.65f, 0f, 0.73f, 1f));
    }


    private bool playerReady;
    private GameObject[] playerCannons; // reference to the player's cannon
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    public ColorList[] _colorlist = new ColorList[10];

    

    void Start()
    {
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
        }
        
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
        GameObject.Find("JoinText" + pId).GetComponent<Text>().enabled = true; //the player's 'Press 'A' to join text'
    }

    public void UpdateColour(int cIdx, int pId) //if the player tries to change their colour, it will be updated here. Also, if the player joins too. 
    {
        playerCannons[pId].transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Laser colour
        playerCannons[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = _colorlist[cIdx]._color; //Trail renderer colour
        GameObject.Find("Player" + (pId + 1) + " Overlay").GetComponent<SpriteRenderer>().color = _colorlist[cIdx]._color; //Overlay colour
        playerCannons[pId].GetComponentInChildren<SpriteRenderer>().color = _colorlist[cIdx]._color; //Cannon colour
        _colorlist[cIdx].isAvailable = false; //making sure other players cannot use the same colour
    }

}
