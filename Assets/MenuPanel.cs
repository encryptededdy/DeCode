using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    public Button mainMenuButton;
    public Text idText;
    public int levelId;
    
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            LevelSwitchStatisticsManager.Instance.SwitchLevel(levelId, 0);
        });
        LevelSwitchStatisticsManager.Instance.UpdateUserId(idText);
    }
}
