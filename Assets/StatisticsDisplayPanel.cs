using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsDisplayPanel : MonoBehaviour
{
    public Text LevelTimes;
    public Text LevelTries;
    public Text QuestionTries;
    
    // Start is called before the first frame update
    void Start()
    {
        LevelTimes.text = string.Join(", ", LevelSwitchStatisticsManager.Instance.LevelTimes);
        LevelTries.text = string.Join(", ", LevelSwitchStatisticsManager.Instance.ResetCounter);
        QuestionTries.text = string.Join(", ", LevelSwitchStatisticsManager.Instance.QuestionCounter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
