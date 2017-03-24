using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public GameObject TimedGM;

    public void LoadByIndex(int sceneIndex)
    {
        Instantiate(TimedGM, Vector3.zero, new Quaternion(0, 0, 0, 0));
        SceneManager.LoadScene(sceneIndex);
    }
}
