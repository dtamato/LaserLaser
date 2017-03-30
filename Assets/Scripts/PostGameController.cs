using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Rewired;

public class PostGameController : MonoBehaviour
{
    #region Variables

    private BaseGM gameManager;
    private string gameType;

    //Player variables.
    private Player[] players;
    private bool[] readyPlayers = new bool[4];
    private int readyCount;

    //Scoring storage.
    private int[] totalScore = new int[4];
    private int[] diamondScore = new int[4];
    private int[] longshotScore = new int[4];
    private int[] trickshotScore = new int[4];
    private int[] doubleshotScore = new int[4];
    private int[] tripleshotScore = new int[4];

    //Variables used in score calculations.
    [SerializeField]
    private int[] diamondWinner, longshotWinner, trickshotWinner, doubleshotWinner, tripleshotWinner;
    private int finalWinner;
    private int highestScore;
    private int highestDiamondScore;
    public int bonusScore;

    //Coroutine variables.
    private float backgroundHeight = 800;     //Maximum height of the bar.
    private float pointHeight;                //Height per point.
    private float distFromCanvasEdge = 0.0f;
    private bool resultsDisplayed = false;

    private float briefPause = 0.1f;
    private float midPause = 0.5f;
    private float longPause = 1.0f;
    private float timeScalar = 50.0f;

    //Text references.
    public Text winningText;
    public List<Text> diamondText;
    public List<Text> longshotText;
    public List<Text> trickshotText;
    public List<Text> doubleshotText;
    public List<Text> tripleshotText;

    //Object references.
    public List<Image> backgrounds;
    public List<GameObject> overlays;

    #endregion

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

        finalScoring();
        displayResults();
	}

    void Update()
    {
        //Only pressable once results have been displayed.
        if (resultsDisplayed)
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

    void finalScoring()
    {
        //Add bonus points for winners of each category to determine highest total score.
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < longshotWinner.Length; j++)
                if (longshotWinner[j] == i)
                    totalScore[i] += bonusScore;

            for (int j = 0; j < trickshotWinner.Length; j++)
                if (trickshotWinner[j] == i)
                    totalScore[i] += bonusScore;

            for (int j = 0; j < doubleshotWinner.Length; j++)
                if (doubleshotWinner[j] == i)
                    totalScore[i] += bonusScore;

            for (int j = 0; j < tripleshotWinner.Length; j++)
                if (tripleshotWinner[j] == i)
                    totalScore[i] += bonusScore;
        }

        //What is the highest score value.
        highestScore = 0;
        highestDiamondScore = 0;
        for (int i = 0; i < 4; i++)
        {
            if (totalScore[i] > highestScore)
            {
                highestScore = totalScore[i];
                finalWinner = i;
            }
            if (diamondScore[i] > highestDiamondScore)
                highestDiamondScore = diamondScore[i];

            //Take the bonus points away, makes it easier to calculate the height of bars in barTick().
            for (int j = 0; j < longshotWinner.Length; j++)
                if (longshotWinner[j] == i)
                    totalScore[i] -= bonusScore;

            for (int j = 0; j < trickshotWinner.Length; j++)
                if (trickshotWinner[j] == i)
                    totalScore[i] -= bonusScore;

            for (int j = 0; j < doubleshotWinner.Length; j++)
                if (doubleshotWinner[j] == i)
                    totalScore[i] -= bonusScore;

            for (int j = 0; j < tripleshotWinner.Length; j++)
                if (tripleshotWinner[j] == i)
                    totalScore[i] -= bonusScore;
        }

        //Used to increment the bar height.
        pointHeight = backgroundHeight / highestScore;
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

        winningText.text = "Player " + (finalWinner + 1) + " Wins!";

        //Raise the bars.
        StartCoroutine("barTick");
    }

    IEnumerator barTick()
    {
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < (highestDiamondScore * timeScalar); i++)
        {
            for (int j = 0; j < 4; j++)
                if ((diamondScore[j] * timeScalar) > i)
                    backgrounds[j].rectTransform.SetInsetAndSizeFromParentEdge
                        (RectTransform.Edge.Bottom, distFromCanvasEdge, ((i + 1) * pointHeight / timeScalar));

            yield return new WaitForSeconds(briefPause / (50 * timeScalar));
        }

        for (int i = 0; i < 4; i++)
            diamondText[i].gameObject.SetActive(true);
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < 4; i++)
            diamondText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show longshot results.
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < longshotWinner.Length; j++)
                if (longshotWinner[j] == i)
                    totalScore[i] += bonusScore;

        for (int i = 0; i < 4; i++)
            longshotText[i].gameObject.SetActive(true);
        if (longshotWinner[0] != -1)
            for (int i = 0; i < longshotWinner.Length; i++)
                backgrounds[longshotWinner[i]].rectTransform.SetInsetAndSizeFromParentEdge
                    (RectTransform.Edge.Bottom, distFromCanvasEdge, (totalScore[longshotWinner[i]] * pointHeight));
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < 4; i++)
            longshotText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show trickshot results.
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < trickshotWinner.Length; j++)
                if (trickshotWinner[j] == i)
                    totalScore[i] += bonusScore;

        for (int i = 0; i < 4; i++)
            trickshotText[i].gameObject.SetActive(true);
        if (trickshotWinner[0] != -1)
            for (int i = 0; i < trickshotWinner.Length; i++)
                backgrounds[trickshotWinner[i]].rectTransform.SetInsetAndSizeFromParentEdge
                    (RectTransform.Edge.Bottom, distFromCanvasEdge, (totalScore[trickshotWinner[i]] * pointHeight));
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < 4; i++)
            trickshotText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show doubleshot results.
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < doubleshotWinner.Length; j++)
                if (doubleshotWinner[j] == i)
                    totalScore[i] += bonusScore;

        for (int i = 0; i < 4; i++)
            doubleshotText[i].gameObject.SetActive(true);
        if (doubleshotWinner[0] != -1)
            for (int i = 0; i < doubleshotWinner.Length; i++)
                backgrounds[doubleshotWinner[i]].rectTransform.SetInsetAndSizeFromParentEdge
                    (RectTransform.Edge.Bottom, distFromCanvasEdge, (totalScore[doubleshotWinner[i]] * pointHeight));
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < 4; i++)
            doubleshotText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show tripleshot results.
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < tripleshotWinner.Length; j++)
                if (tripleshotWinner[j] == i)
                    totalScore[i] += bonusScore;

        for (int i = 0; i < 4; i++)
            tripleshotText[i].gameObject.SetActive(true);
        if (tripleshotWinner[0] != -1)
            for (int i = 0; i < tripleshotWinner.Length; i++)
                backgrounds[tripleshotWinner[i]].rectTransform.SetInsetAndSizeFromParentEdge
                    (RectTransform.Edge.Bottom, distFromCanvasEdge, (totalScore[tripleshotWinner[i]] * pointHeight));
        yield return new WaitForSeconds(longPause);
        for (int i = 0; i < 4; i++)
            tripleshotText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show the final winner
        winningText.gameObject.SetActive(true);
        resultsDisplayed = true;
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