using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace CustomUI
{
    public delegate void QuestionScreenCallback(int attempts);
    public class QuestionScreenLogic : MonoBehaviour
    {
        public Button[] OptionButtons;
        public Button NextButton;
        public GameObject Prefab;
        public Text Title;
        public Text Description;
        
        private int _noAttempts;
        
        public void ShowNewQuestion(string question, string correctAnswer, string[] otherAnswers, string correctAnswerDesc,
            QuestionScreenCallback questionScreenCallback)
        {
            if (!Prefab.activeInHierarchy) Prefab.SetActive(true);
            NextButton.gameObject.SetActive(false);
            Description.text = "";
            _noAttempts = 0;
            
            NextButton.onClick.RemoveAllListeners();
            foreach (var optionButton in OptionButtons)
            {
                optionButton.onClick.RemoveAllListeners();
                optionButton.GetComponent<Image>().color = Color.white;
            }

            NextButton.onClick.AddListener(() =>
            {
                Prefab.SetActive(false);
                questionScreenCallback(_noAttempts);
            });
            
            Title.text = question;

            var correctId = new Random().Next(0, 3);
            var correctButton = OptionButtons[correctId];

            correctButton.GetComponentInChildren<Text>().text = correctAnswer;
            correctButton.onClick.AddListener((() =>
            {
                _noAttempts++;
                NextButton.gameObject.SetActive(true);
                // Show description
                Description.text = correctAnswerDesc;
                correctButton.GetComponent<Image>().color = new Color(0.55f, 0.8f, 0.42f);
            }));

            var questionsVisited = 0;
            
            for (var i = 0; i < 4; i++)
            {
                if (i == correctId) continue;
                var answer = otherAnswers[questionsVisited++];
                var thisButton = OptionButtons[i];
                thisButton.GetComponentInChildren<Text>().text = answer;
                thisButton.onClick.AddListener((() =>
                {
                    _noAttempts++;
                    thisButton.GetComponent<Image>().color = new Color(0.77f, 0.34f, 0.2f);
                }));
            }

        }
    }
}