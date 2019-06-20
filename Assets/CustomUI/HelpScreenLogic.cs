using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HelpScreenLogic : MonoBehaviour
{
    public GameObject Page2;
    public GameObject Page1;
    public GameObject Background;
    public Button NextButton;

    private bool onPage2 = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Hide the second page, show the first one...
        Page2.SetActive(false);
        Page1.SetActive(true);
        Background.SetActive(true);
        NextButton.onClick.AddListener(nextButtonClickHandler);
    }

    private void nextButtonClickHandler()
    {
        if (onPage2)
        {
            Page2.SetActive(false);
            Page1.SetActive(false);
            NextButton.gameObject.SetActive(false);
            Background.SetActive(false);
        }
        else
        {
            // On page 1
            onPage2 = true;
            Page2.SetActive(true);
            Page1.SetActive(false);
        }
    }
}
