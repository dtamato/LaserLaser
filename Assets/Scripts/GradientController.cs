using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GradientController : MonoBehaviour
{

    [SerializeField] private Color colourTop;

    void Start()
    {
        gameObject.GetComponent<Image>().color = colourTop;
    }

    void Update()
    {
        
    }
}
