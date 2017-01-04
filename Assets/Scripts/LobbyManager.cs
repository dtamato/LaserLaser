using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Rewired;

public class LobbyManager : MonoBehaviour {

    
    //[SerializeField] int playerId = 0;

 
    private bool playerReady;
    private GameObject[] playerCannons; // reference to the player's cannon
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;


    void Start()
    {
        playerCannons = new GameObject[4]; 
        playerCannons[0] = player1;
        playerCannons[1] = player2;
        playerCannons[2] = player3;
        playerCannons[3] = player4;

        for (int i = 0; i < playerCannons.Length; i++)
        {
            playerCannons[i].GetComponentInChildren<SpriteRenderer>().color = Color.gray;
            playerCannons[i].transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
            playerCannons[i].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
        }
        
    }

    public int IncrementIndex(int idx) //if the player wants to change their colour forward ->
    {
        GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[idx].isAvailable = true; //accessing the array of available colours contained within ColorManager
        do
        {
            idx++;
            idx %= 10;
        } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[idx].isAvailable);
        return idx;
    }

    public int DecrementIndex(int idx) //if the player wants to change their colour backward <-
    {
        GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[idx].isAvailable = true;
        do
        {
            idx = (idx - 1) % 10;
            idx = idx < 0 ? idx + 10 : idx; //is check 1 true? if yes, use check 2 (wraps around back to the end of the array when you're decrementing past the first element)
        } while (!GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[idx].isAvailable);
        return idx;
    }

    public void UnjoinColour(int cIdx,int pId) //if the player leaves, then return them to their position, disable their cannon (checked within PlayerActivationCheck()), and change their colour to grey
    {
        playerCannons[pId].GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        playerCannons[pId].transform.Find("Laser").GetComponent<SpriteRenderer>().color = Color.grey;
        playerCannons[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = Color.grey;
        playerCannons[pId].GetComponentInChildren<Rigidbody2D>().isKinematic = false; // adding gravity so the player can fall into place 
        playerCannons[pId].GetComponentInChildren<Rigidbody2D>().transform.position = GameObject.Find("Player" + (pId + 1) + " Overlay").GetComponent<OverlayController>().resetPos; //resetting the player's cannon to its original position
        GameObject.Find("JoinText" + pId).GetComponent<Text>().enabled = true; //the player's 'Press 'A' to join text'
        //GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[playerCannon.GetComponent<Cannon>().colorIdx].isAvailable = true;
    }

    public void UpdateColour(int cIdx, int pId) //if the player tries to change their colour, it will be updated here. Also, if the player joins
    {
        playerCannons[pId].transform.Find("Laser").GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[cIdx]._color;
        playerCannons[pId].transform.Find("Laser").GetComponent<TrailRenderer>().material.color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[cIdx]._color;
        GameObject.Find("Player" + (pId + 1) + " Overlay").GetComponent<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[cIdx]._color;
        playerCannons[pId].GetComponentInChildren<SpriteRenderer>().color = GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[cIdx]._color;
        GameObject.Find("ColorManager").GetComponent<ColorManager>()._colorlist[cIdx].isAvailable = false; //making sure other players cannot use the same colour
    }

}
