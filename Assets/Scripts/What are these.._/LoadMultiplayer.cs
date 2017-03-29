using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMultiplayer : MonoBehaviour
{
    void OnTriggerStay()
    {
        if(Input.GetKey("e"))
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("FFALobby", LoadSceneMode.Additive);
    }
}
