using System;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomUI
{
    public class MainMenuLogic: MonoBehaviour
    {
        public Button StartButton;
        public Button ArrayButton;
        public Button ListButton;
        public Button QueueButton;
        public Button StackButton;

        private void Start()
        {
            StartButton.onClick.AddListener(() => LevelSwitchStatisticsManager.Instance.SwitchLevel(0, 1));
            ArrayButton.onClick.AddListener(() => LevelSwitchStatisticsManager.Instance.SwitchLevel(0, 1));
            ListButton.onClick.AddListener(() => LevelSwitchStatisticsManager.Instance.SwitchLevel(0, 2));
            QueueButton.onClick.AddListener(() => LevelSwitchStatisticsManager.Instance.SwitchLevel(0, 3));
            StackButton.onClick.AddListener(() => LevelSwitchStatisticsManager.Instance.SwitchLevel(0, 4));
        }
    }
}