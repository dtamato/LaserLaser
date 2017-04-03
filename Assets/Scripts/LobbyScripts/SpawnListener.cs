﻿using UnityEngine.UI;
using UnityEngine;
using Rewired;

/// <summary>
/// Attached to spawnpoint in MergedMain scene, listens for attached controllers to press A.
/// When they press A, a cannon is spawned and the player has control of it.
/// Once the join time has expired, the spawn point is deactivated.
/// </summary>

public class SpawnListener : MonoBehaviour
{
    public GameObject playerRef;
    public int playerID;
    public bool taken;

    private BaseGM gameManager;
    private Player rewiredPlayer;
    
    void Awake ()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGM>();
        rewiredPlayer = ReInput.players.GetPlayer(playerID);
    }
	
	void Update ()
    {
        if (rewiredPlayer.GetButtonDown("Fire"))
        {
            if (!taken)
            {
                GameObject obj = Instantiate(playerRef, transform.position, transform.rotation);
                obj.GetComponent<Cannon>().SetID(playerID);
                obj.GetComponent<Cannon>().spawnPoint = this.gameObject;
                obj.GetComponentInChildren<Laser>().setGameMode(gameManager.gameMode);
                obj.GetComponent<Cannon>().SetPauseMenu(gameManager.GetPauseMenu());
                gameManager.playerCount++;
                gameManager.activePlayers[playerID] = true;
                taken = true;
            }
        }
        
        if (gameManager.getState() == BaseGM.GAMESTATE.INGAME)
        {
            this.gameObject.SetActive(false);
        }
    }
}
