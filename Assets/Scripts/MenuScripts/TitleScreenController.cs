using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TitleScreenController : MonoBehaviour {
	
	void Update () {
	
		if (ReInput.players.GetPlayer (0).GetButtonDown("Fire")) 
        {
            gameObject.GetComponent<LoadSceneOnClick>().LoadByIndex(1);
        }
	}
}
