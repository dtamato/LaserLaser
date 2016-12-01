using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GradientController : MonoBehaviour {

    [SerializeField] private Color colourTop = Color.blue;
    public Color colourBottom = Color.black;
    public Color newColour;


    public Material yourGradientMaterial;

    void Start()
    {
        
        yourGradientMaterial.SetColor("_Color2", Color.black); // the bottom color
    }

    void Update()
    {
        yourGradientMaterial.SetColor("_Color", colourTop); // the top color
    }
}
