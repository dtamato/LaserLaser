﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Rewired;

public class PostGameController : MonoBehaviour
{
    private BaseGM gameManager;
    private string gameType;

    private Player[] players;
    private bool[] readyPlayers = new bool[4];
    private int readyCount;

    private int[] totalScore = new int[4];
    private int[] diamondScore = new int[4];
    private int[] longshotScore = new int[4];
    private int[] trickshotScore = new int[4];
    private int[] doubleshotScore = new int[4];
    private int[] tripleshotScore = new int[4];

    //Arrays used in case of tie / multiple winners in each category.
    private int[] diamondWinner, longshotWinner, trickshotWinner, doubleshotWinner, tripleshotWinner;

    public List<Text> diamondText;
    public List<Text> longshotText;
    public List<Text> trickshotText;
    public List<Text> doubleshotText;
    public List<Text> tripleshotText;

    public List<Image> backgrounds;
    public List<GameObject> overlays;
    public List<RectTransform> scorebars;

    void Start ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        gameType = gameManager.gameType;

        players = new Player[4];
        for (int i = 0; i < 4; i++)
        {
            players[i] = ReInput.players.GetPlayer(i);
            backgrounds[i].color = gameManager.playerColors[i];
            overlays[i].GetComponent<Image>().color = gameManager.playerColors[i];

            if (backgrounds[i].color == Color.grey)
            {
                overlays[i].SetActive(true);
                readyPlayers[i] = true;
                readyCount++;
            }
            else
            {
                readyPlayers[i] = false;
                overlays[i].SetActive(false);
            }
        }

        importScores();
        diamondCount();

        if (gameType == "TIMED")
        {
            longshotCount();
            trickshotCount();
            doubleshotCount();
            tripleshotCount();
        }

        displayResults();
	}

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i].GetButtonDown("StartGame") && readyPlayers[i] == false)
            {
                overlays[i].SetActive(true);
                readyPlayers[i] = true;
                readyCount++;
            }

            if (players[i].GetButtonDown("Back") && readyPlayers[i] == true)
            {
                overlays[i].SetActive(false);
                readyPlayers[i] = false;
                readyCount--;
            }
        }

        if (readyCount == 4)
            gameManager.returnToMenu();
    }

    void importScores()
    {
        for (int i = 0; i < 4; i++)
        {
            totalScore[i] = gameManager.playerScores[i];
            diamondScore[i] = gameManager.playerScores[i];
            longshotScore[i] = gameManager.longshots[i];
            trickshotScore[i] = gameManager.trickshots[i];
            doubleshotScore[i] = gameManager.doubleshots[i];
            tripleshotScore[i] = gameManager.tripleshots[i];
        }
    }

    //Adjust all the text.
    void displayResults()
    {
        for (int i = 0; i < 4; i++)
        {
            diamondText[i].text = "Diamonds Hit: " + diamondScore[i];
            longshotText[i].text = "Longshots: " + longshotScore[i];
            trickshotText[i].text = "Trickshots: " + trickshotScore[i];
            doubleshotText[i].text = "Doubleshots: " + doubleshotScore[i];
            tripleshotText[i].text = "Tripleshots: " + tripleshotScore[i];
        }

        if (diamondWinner[0] != -1)
            for (int i = 0; i < diamondWinner.Length; i++)
                diamondText[diamondWinner[i]].color = Color.yellow;

        if (longshotWinner[0] != -1)
            for (int i = 0; i < longshotWinner.Length; i++)
                longshotText[longshotWinner[i]].color = Color.yellow;

        if (trickshotWinner[0] != -1)
            for (int i = 0; i < trickshotWinner.Length; i++)
                trickshotText[trickshotWinner[i]].color = Color.yellow;

        if (doubleshotWinner[0] != -1)
            for (int i = 0; i < doubleshotWinner.Length; i++)
                doubleshotText[doubleshotWinner[i]].color = Color.yellow;

        if (tripleshotWinner[0] != -1)
            for (int i = 0; i < tripleshotWinner.Length; i++)
                tripleshotText[tripleshotWinner[i]].color = Color.yellow;
    }

    //Base score metric.
    void diamondCount()
    {
        int hi = 0;
        int loneWinner = 0;
        int[] tiedWinner = new int[4];
        int tie = 1;

        //Check player's diamond scores.
        for (int i = 0; i < 4; i++)
        {
            //If a player has more than anyone else, they are the lone winner.
            if (diamondScore[i] > hi)
            {
                hi = diamondScore[i];
                loneWinner = i;
                tie = 0;
                tiedWinner[tie] = i;
            }
            //If a player ties, they are added to the tied winner list.
            else if (diamondScore[i] >= hi && diamondScore[i] != 0)
            {
                tie++;
                tiedWinner[tie] = i;
            }
            else if (diamondScore[i] >= hi && diamondScore[i] == 0)
            {
                tiedWinner[0] = -1;
            }
        }

        //Record the winner in the array.
        if (tie == 0)
        {
            diamondWinner = new int[1];
            diamondWinner[0] = loneWinner;
        }
        else if (tiedWinner[0] == -1)
        {
            diamondWinner = new int[1];
            diamondWinner[0] = -1;
        }
        else
        {
            diamondWinner = new int[tie + 1];
            for (int i = 0; i <= tie; i++)
                diamondWinner[i] = tiedWinner[i];
        }
    }

    //4 similiar functions to calculate winners of each category.
    //Only impacts score in timed mode.
    void longshotCount()
    {
        int hi = 0;
        int loneWinner = 0;
        int[] tiedWinner = new int[4];
        int tie = 1;

        for (int i = 0; i < 4; i++)
        {
            if (longshotScore[i] > hi)
            {
                hi = longshotScore[i];
                loneWinner = i;
                tie = 0;
                tiedWinner[tie] = i;
            }
            else if (longshotScore[i] >= hi && longshotScore[i] != 0)
            {
                tie++;
                tiedWinner[tie] = i;
            }
            else if (longshotScore[i] >= hi && longshotScore[i] == 0)
            {
                tiedWinner[0] = -1;
            }
        }

        if (tie == 0)
        {
            longshotWinner = new int[1];
            longshotWinner[0] = loneWinner;
        }
        else if (tiedWinner[0] == -1)
        {
            longshotWinner = new int[1];
            longshotWinner[0] = -1;
        }
        else 
        {
            longshotWinner = new int[tie + 1];
            for (int i = 0; i <= tie; i++)
                longshotWinner[i] = tiedWinner[i];
        }
    }

    void trickshotCount()
    {
        int hi = 0;
        int loneWinner = 0;
        int[] tiedWinner = new int[4];
        int tie = 1;

        for (int i = 0; i < 4; i++)
        {
            if (trickshotScore[i] > hi)
            {
                hi = trickshotScore[i];
                loneWinner = i;
                tie = 0;
                tiedWinner[tie] = i;
            }
            else if (trickshotScore[i] >= hi && trickshotScore[i] != 0)
            {
                tie++;
                tiedWinner[tie] = i;
            }
            else if (trickshotScore[i] >= hi && trickshotScore[i] == 0)
            {
                tiedWinner[0] = -1;
            }
        }

        if (tie == 0)
        {
            trickshotWinner = new int[1];
            trickshotWinner[0] = loneWinner;
        }
        else if (tiedWinner[0] == -1)
        {
            trickshotWinner = new int[1];
            trickshotWinner[0] = -1;
        }
        else
        {
            trickshotWinner = new int[tie + 1];
            for (int i = 0; i <= tie; i++)
                trickshotWinner[i] = tiedWinner[i];
        }
    }

    void doubleshotCount()
    {
        int hi = 0;
        int loneWinner = 0;
        int[] tiedWinner = new int[4];
        int tie = 1;

        for (int i = 0; i < 4; i++)
        {
            if (doubleshotScore[i] > hi)
            {
                hi = doubleshotScore[i];
                loneWinner = i;
                tie = 0;
                tiedWinner[tie] = i;
            }
            else if (doubleshotScore[i] >= hi && doubleshotScore[i] != 0)
            {
                tie++;
                tiedWinner[tie] = i;
            }
            else if (doubleshotScore[i] >= hi && doubleshotScore[i] == 0)
            {
                tiedWinner[0] = -1;
            }
        }

        if (tie == 0)
        {
            doubleshotWinner = new int[1];
            doubleshotWinner[0] = loneWinner;
        }
        else if (tiedWinner[0] == -1)
        {
            doubleshotWinner = new int[1];
            doubleshotWinner[0] = -1;
        }
        else
        {
            doubleshotWinner = new int[tie + 1];
            for (int i = 0; i <= tie; i++)
                doubleshotWinner[i] = tiedWinner[i];
        }
    }

    void tripleshotCount()
    {
        int hi = 0;
        int loneWinner = 0;
        int[] tiedWinner = new int[4];
        int tie = 1;

        for (int i = 0; i < 4; i++)
        {
            if (tripleshotScore[i] > hi)
            {
                hi = tripleshotScore[i];
                loneWinner = i;
                tie = 0;
                tiedWinner[tie] = i;
            }
            else if (tripleshotScore[i] >= hi && tripleshotScore[i] != 0)
            {
                tie++;
                tiedWinner[tie] = i;
            }
            else if (tripleshotScore[i] >= hi && tripleshotScore[i] == 0)
            {
                tiedWinner[0] = -1;
            }
        }

        if (tie == 0)
        {
            tripleshotWinner = new int[1];
            tripleshotWinner[0] = loneWinner;
        }
        else if (tiedWinner[0] == -1)
        {
            tripleshotWinner = new int[1];
            tripleshotWinner[0] = -1;
        }
        else
        {
            tripleshotWinner = new int[tie + 1];
            for (int i = 0; i <= tie; i++)
                tripleshotWinner[i] = tiedWinner[i];
        }
    }
}
