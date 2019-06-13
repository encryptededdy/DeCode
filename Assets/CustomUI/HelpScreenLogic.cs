using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreenLogic : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;
    public Button nextButton;

    private bool onPage2 = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Hide the second page, show the first one...
        page2.SetActive(false);
        page1.SetActive(true);
        nextButton.onClick.AddListener(nextButtonClickHandler);
    }

    private void nextButtonClickHandler()
    {
        if (onPage2)
        {
            page2.SetActive(false);
            page1.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            // On page 1
            onPage2 = true;
            page2.SetActive(true);
            page1.SetActive(false);
        }
    }
}
