using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject defaultButton;

    private bool buttonSelected;

    
    void Start()
    {
		EventSystem.current.SetSelectedGameObject(defaultButton);
    }
	
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            EventSystem.current.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
    }

    private void OnDisable()
    {
        buttonSelected = false;
    }
}