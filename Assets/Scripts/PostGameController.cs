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
    private bool gameTied = false;
    private int finalWinner;
    private int highestScore;
    private int highestDiamondScore;
    public int bonusScore;

    //Coroutine variables.
    private float fillPerDiamond;
    private float fillPerBonus;
    private bool resultsDisplayed = false;
    private float barScalarShort = 150.0f;
    private float barScalarLong = 300.0f;
    private float briefPause = 0.1f;
    private float midPause = 0.5f;
    private float longPause = 1f;

    //Text references.
    public List<Text> winningText;
    public List<Text> diamondText;
    public List<Text> longshotText;
    public List<Text> trickshotText;
    public List<Text> doubleshotText;
    public List<Text> tripleshotText;

    //Object references.
    public List<Image> backgrounds;
    public List<GameObject> readyTexts;

	[SerializeField] AudioClip applauseAudioClip;

    #endregion

    void Start ()
    {
		GameObject cameraOverlay = GameObject.Find("Camera Overlay").gameObject;
		cameraOverlay.GetComponent<Image>().color = Color.black;
		cameraOverlay.GetComponent<FadeCameraOverlay>().FadeOut();

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        gameType = gameManager.gameType;

        players = new Player[4];
        for (int i = 0; i < 4; i++)
        {
            players[i] = ReInput.players.GetPlayer(i);
            backgrounds[i].color = gameManager.playerColors[i];

			if (gameManager.activePlayers[i] == false)
            {
                readyPlayers[i] = true;
                readyCount++;
            }
            else
            {
                readyPlayers[i] = false;
                readyTexts[i].SetActive(false);
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
					readyTexts[i].GetComponent<Text>().text = "READY!";
                    readyPlayers[i] = true;
                    readyCount++;
					if (readyCount == 4) { gameManager.changeScene(gameManager.creditsSceneIndex); }
                }

                if (players[i].GetButtonDown("Back") && readyPlayers[i] == true)
                {
					readyTexts[i].GetComponent<Text>().text = "PRESS START";
                    readyPlayers[i] = false;
                    readyCount--;
                }
            }
        }
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
                gameTied = false;
                finalWinner = i;
            }
            else if (totalScore[i] == highestScore)
            {
                gameTied = true;
            }

            if (diamondScore[i] > highestDiamondScore)
                highestDiamondScore = diamondScore[i];
        }

        //Percent of bar filled per point. Scaled based on highest score of game.
        if (highestDiamondScore >= 36)
            fillPerDiamond = 0.75f / highestDiamondScore;
        else if (highestDiamondScore >= 12)
            fillPerDiamond = 0.50f / highestDiamondScore;
        else if (highestDiamondScore >= 4)
            fillPerDiamond = 0.25f / highestDiamondScore;
        else
            fillPerDiamond = 0.07f / highestDiamondScore;

        fillPerBonus = fillPerDiamond * bonusScore;
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

        //Raise the bars.
        StartCoroutine("barTick");
    }

    IEnumerator barTick()
    {
        //Set all bars to the bottom.
        for (int i = 0; i < 4; i++)
            backgrounds[i].fillAmount = 0.0f;

        yield return new WaitForSeconds(longPause);

        for (int i = 0; i < barScalarLong; i++)
        {
            for (int j = 0; j < 4; j++)
                if (backgrounds[j].fillAmount < fillPerDiamond * diamondScore[j])
                    backgrounds[j].fillAmount += (fillPerDiamond * diamondScore[j] / barScalarLong);
			if(this.GetComponent<AudioSource>().isPlaying == false) { this.GetComponent<AudioSource>().Play(); }
            yield return new WaitForSeconds(midPause / barScalarLong);
        }

        for (int i = 0; i < 4; i++)
            if (gameManager.activePlayers[i] == true)
                diamondText[i].gameObject.SetActive(true);
        yield return new WaitForSeconds(midPause);

        for (int i = 0; i < 4; i++)
            if (gameManager.activePlayers[i] == true)
                diamondText[i].gameObject.SetActive(false);
        yield return new WaitForSeconds(midPause);

        //Show longshot results.
        if (longshotWinner[0] != -1)
        {
            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    longshotText[i].gameObject.SetActive(true);

            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "Longshot Master";
                winningText[i].gameObject.SetActive(true);
            }

            if (longshotWinner[0] != -1)
                for (int j = 0; j < barScalarShort; j++)
                {
                    for (int i = 0; i < longshotWinner.Length; i++)
                        backgrounds[longshotWinner[i]].fillAmount += (fillPerBonus / barScalarShort);

					if(this.GetComponent<AudioSource>().isPlaying == false) { this.GetComponent<AudioSource>().Play(); }
                    yield return new WaitForSeconds(midPause / barScalarShort);
                }
            yield return new WaitForSeconds(midPause);

            for (int i = 0; i < winningText.Count; i++)
                winningText[i].gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    longshotText[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(midPause);
        }

        //Show trickshot results.
        if (trickshotWinner[0] != -1)
        {
            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    trickshotText[i].gameObject.SetActive(true);

            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "Trickshot Titan";
                winningText[i].gameObject.SetActive(true);
            }

            if (trickshotWinner[0] != -1)
                for (int j = 0; j < barScalarShort; j++)
                {
                    for (int i = 0; i < trickshotWinner.Length; i++)
                        backgrounds[trickshotWinner[i]].fillAmount += (fillPerBonus / barScalarShort);

					if(this.GetComponent<AudioSource>().isPlaying == false) { this.GetComponent<AudioSource>().Play(); }
                    yield return new WaitForSeconds(midPause / barScalarShort);
                }
            yield return new WaitForSeconds(midPause);

            for (int i = 0; i < winningText.Count; i++)
                winningText[i].gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    trickshotText[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(midPause);
        }

        //Show doubleshot results.
        if (doubleshotWinner[0] != -1)
        {
            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    doubleshotText[i].gameObject.SetActive(true);

            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "Doubleshot Pro";
                winningText[i].gameObject.SetActive(true);
            }

            if (doubleshotWinner[0] != -1)
                for (int j = 0; j < barScalarShort; j++)
                {
                    for (int i = 0; i < doubleshotWinner.Length; i++)
                        backgrounds[doubleshotWinner[i]].fillAmount += (fillPerBonus / barScalarShort);

					if(this.GetComponent<AudioSource>().isPlaying == false) { this.GetComponent<AudioSource>().Play(); }
                    yield return new WaitForSeconds(midPause / barScalarShort);
                }
            yield return new WaitForSeconds(midPause);

            for (int i = 0; i < winningText.Count; i++)
                winningText[i].gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    doubleshotText[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(midPause);
        }

        //Show tripleshot results.
        if (tripleshotWinner[0] != -1)
        {
            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    tripleshotText[i].gameObject.SetActive(true);

            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "The Trifecta Captain";
                winningText[i].gameObject.SetActive(true);
            }

            if (tripleshotWinner[0] != -1)
                for (int j = 0; j < barScalarShort; j++)
                {
                    for (int i = 0; i < tripleshotWinner.Length; i++)
                        backgrounds[tripleshotWinner[i]].fillAmount += (fillPerBonus / barScalarShort);

					if(this.GetComponent<AudioSource>().isPlaying == false) { this.GetComponent<AudioSource>().Play(); }
                    yield return new WaitForSeconds(midPause / barScalarShort);
                }
            yield return new WaitForSeconds(midPause);

            for (int i = 0; i < winningText.Count; i++)
                winningText[i].gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
                if (gameManager.activePlayers[i] == true)
                    tripleshotText[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(midPause);
        }

        //Show the final winner
		if (gameTied) {
			
            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "A Tie Of Epic Proportions!";
                winningText[i].gameObject.SetActive(true);
            }
		}
		else {
			
            for (int i = 0; i < winningText.Count; i++)
            {
                winningText[i].text = "Player " + (finalWinner + 1) + " Wins!";
                winningText[i].gameObject.SetActive(true);
            }
		}

		// Show "Press Start" text for active players
		for(int i = 0; i < players.Length; i++) {

			if(gameManager.activePlayers[i] == true) {

				readyTexts[i].SetActive(true);
			}
		}

        resultsDisplayed = true;
		this.GetComponent<AudioSource>().clip = applauseAudioClip;
		this.GetComponent<AudioSource>().Play();
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