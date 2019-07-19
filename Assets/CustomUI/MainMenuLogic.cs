using System;
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
            StartButton.onClick.AddListener(() => SceneManager.LoadScene("Array"));
            ArrayButton.onClick.AddListener(() => SceneManager.LoadScene("Array"));
            ListButton.onClick.AddListener(() => SceneManager.LoadScene("List"));
            QueueButton.onClick.AddListener(() => SceneManager.LoadScene("Queue"));
            StackButton.onClick.AddListener(() => SceneManager.LoadScene("Stack"));
        }
    }
}