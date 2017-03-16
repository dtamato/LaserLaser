using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Attached to spawnpoint in MergedMain scene, listens for attached controllers to press A.
/// When they press A, a cannon is spawned and the player has control of it.
/// Once the join time has expired, the spawn point is destroyed.
/// </summary>

public class SpawnListener : MonoBehaviour
{
    public GameObject playerRef;
    public int playerID;

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
            Instantiate(playerRef, transform.position, transform.rotation);
            gameManager.playerCount++;
        }
        
        if (gameManager.joinGameDelay <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
