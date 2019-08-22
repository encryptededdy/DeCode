using System.Collections.Generic;
using System.Linq;
using CustomUI;
using LevelManager;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ListQuestionController : MonoBehaviour
    {
        public ListOperationQueue OperationQueue;
        private Queue<ListQuestionData> _questions = new Queue<ListQuestionData>();
        public Text Title;
        public Text Description;
        public Button GreenButton;
        public Button OrangeButton;

        public QuestionScreenLogic QuestionScreenLogic;

        private ListQuestionData _currentQuestion;

        private int _currentQuestionResets = 0;

        private void Start()
        {
            OperationQueue.HideAllOperations();
            OperationQueue.ShowOperation(ListOperations.Add);
            OperationQueue.ShowOperation(ListOperations.CopyOne);
            
            // Define questions...
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add",
                "Expand the array to size 2 then add 2 vehicles to fill the two slots.",
                new List<VehicleType>()
                {
                    VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    foreach (var obj in answer)
                    {
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }
                    return correct && answer.Count == 2;
                }
             ));
            
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add Again",
                "Add 1 more vehicle. To do this, you need to extend the array by 1 again.",
                new List<VehicleType>()
                {
                    VehicleType.empty, VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    foreach (var obj in answer)
                    {
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }
                    return correct && answer.Count == 3;
                },
                () =>
                {
                    QuestionScreenLogic.ShowNewQuestion("If we need a list that can get larger, we could just implement it by having a massive array (e.g size 2147483647) instead of doing the slow expansion operation every time. What is one reason why we might not?.",
                        "High memory usage",
                        new []{"Slow insertion performance", "Slow retrieval performance", "Hard to sort"},
                        "Correct - we don't use a massive array since it would use up too much memory, especially if we aren't using the full size.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(3, attempts));
                }
            ));
            
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add Again 2",
                "Add 1 more vehicle. To do this, you need to extend the array by 1 again.",
                new List<VehicleType>()
                {
                    VehicleType.empty, VehicleType.empty, VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    foreach (var obj in answer)
                    {
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }
                    return correct && answer.Count == 4;
                }
            ));
            
            _questions.Enqueue(new ListQuestionData(
                "Improve Efficiency",
                "We can see that increasing the size by 1 every time is inefficient. Instead, most ArrayList implementations double the size of the array every time it needs to be extended. Let's try that - add 2 vehicles.",
                new List<VehicleType>()
                {
                    VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    foreach (var obj in answer)
                    {
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }
                    return correct && answer.Count == 2;
                },
                () =>
                {
                    OperationQueue.HideAllOperations();
                    OperationQueue.ShowOperation(ListOperations.Add);
                    OperationQueue.ShowOperation(ListOperations.CopyDouble);
                    QuestionScreenLogic.ShowNewQuestion("If we have a list that starts at size 0, and we add elements one-by-one until we reach size 100, how many cars do we end up having to perform the (relatively slow) copy operation on?",
                        "1 + 2 + 3 + ... + 99 = 4950 times",
                        new []{"100 times", "99 times", "101 times"},
                        "Correct. We have to copy one car the going from 1 -> 2, two cars from 2 -> 3 and so on and so forth.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(4, attempts));
                }
            ));
            
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add Again",
                "Now, add another 3 vehicles (sequentially, to indices 2, 3, 4). We can see that the doubling method is much more efficient, as it avoids overusing the expensive copying operation.",
                new List<VehicleType>()
                {
                    VehicleType.empty, VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    for (var index = answer.Count - 4; index >= 0; index--) // Loop skipping the last three...
                    {
                        var obj = answer[index];
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }

                    return correct && answer.Count == 8;
                },
                () =>
                {
                    QuestionScreenLogic.ShowNewQuestion("If we perform the same operation (start at 0, add elements until we reach size 100) but we use doubling instead, how many cars do we end up copying?",
                        "1 + 2 + 4 + 8 + ... + 64 = 127 times",
                        new []{"99 times", "100 x 100 = 10000 times", "50 times"},
                        "Correct - We need to double 7 times. 1 -> 2 -> 4 -> 8 -> 16 -> 32 -> 64 -> 128, thus we copy 1, 2, 4, 8, 16, 32, 64 each time.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(5, attempts));
                }
            ));
            
            QuestionScreenLogic.ShowNewQuestion("Just so we can tell how much you already know - if we consider the ArrayList implementation of a List, what's a difference between it and a plan Array?",
                "The ArrayList can expand (no fixed size)",
                new []{"ArrayLists can be larger than arrays", "ArrayLists do not support random access", "I don't know"},
                "",
                attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(2, attempts),
                false);
                        
            NextQuestion();
        }

        private void NextLevel()
        {
            LevelSwitchStatisticsManager.Instance.SwitchLevel(2, 3);
        }

        private void NextQuestion()
        {
            if (_questions.Count == 0)
            {
                Title.text = "Lists complete!";
                Description.text = "Click Start to continue onto Queues";
                GreenButton.gameObject.SetActive(true);
                GreenButton.GetComponentInChildren<Text>().text = "Start";
                GreenButton.onClick.RemoveAllListeners();
                GreenButton.onClick.AddListener(NextLevel);
                OrangeButton.gameObject.SetActive(false);
                return;
            }

            if (_currentQuestion != null)
            {
                LevelSwitchStatisticsManager.Instance.SendSubData(1, _currentQuestionResets, _currentQuestion.Title);
                _currentQuestionResets = 0;
            }

            _currentQuestion = _questions.Dequeue();

//            if (!OperationQueue.GetArrayState().SequenceEqual(_currentQuestion.InitialState))
//            {
//                // Clear parking lot
//                OperationQueue.QueueOperation(new Queued(ArrayOperations.Reset));
//            }

            // Fill in UI
            Title.text = _currentQuestion.Title;
            Description.text = "Click Start to begin question";
            // Configure buttons
            GreenButton.gameObject.SetActive(true);
            GreenButton.GetComponentInChildren<Text>().text = "Start";
            GreenButton.onClick.RemoveAllListeners();
            GreenButton.onClick.AddListener(StartQuestion);
            
            OrangeButton.gameObject.SetActive(false);

            _currentQuestion.ExecuteBefore?.Invoke();
        }

        private void StartQuestion()
        {
            // Fill in UI
            Title.text = _currentQuestion.Title;
            Description.text = _currentQuestion.Description;
            
            // Clear parking lot and repopulate iff it is needed
            if (!OperationQueue.GetArrayState().SequenceEqual(_currentQuestion.InitialState))
            {
                // Reset Array Size if needed.....
                if (OperationQueue.GetArrayState().Count != _currentQuestion.InitialState.Count)
                {
                    OperationQueue.QueueOperation(new QueuedListOperation(ListOperations.Reset, null));
                    OperationQueue.QueueOperation(new QueuedListOperation(ListOperations.SetSize, _currentQuestion.InitialState.Count, null));
                }
                
                // OperationQueue.QueueOperation(new QueuedListOperation(ListOperations.Reset, null));
                // Add cars
//                for (var i = 0; i < _currentQuestion.InitialState.Count; i++)
//                {
//                    // Work around issue with trying to insert VehicleType.empty
//                    if (!_currentQuestion.InitialState[i].Equals(VehicleType.empty))
//                    {
////                        Unimplemented 
////                        OperationQueue.QueueOperation(new QueuedListOperation(ListOperations.AddSpecific, i,
////                            _currentQuestion.InitialState[i]));
//                    }
//                }
            }

            // Reset log
            OperationQueue.ResetLog();
            
            GreenButton.gameObject.SetActive(true);
            GreenButton.GetComponentInChildren<Text>().text = "Check";
            GreenButton.onClick.RemoveAllListeners();
            GreenButton.onClick.AddListener(CheckQuestion);
            
            OrangeButton.gameObject.SetActive(true);
            OrangeButton.GetComponentInChildren<Text>().text = "Reset";
            OrangeButton.onClick.RemoveAllListeners();
            OrangeButton.onClick.AddListener(ResetQuestion);
        }

        private void CheckQuestion()
        {
//            OperationQueue.GetArrayState().ForEach(obj => print(obj));
            if (_currentQuestion.AnswerChecker(OperationQueue.GetArrayState()))
            {
                // Correct!
                Description.text = "Correct!";
                
                GreenButton.gameObject.SetActive(true);
                GreenButton.GetComponentInChildren<Text>().text = "Next";
                GreenButton.onClick.RemoveAllListeners();
                GreenButton.onClick.AddListener(NextQuestion);
            
                OrangeButton.gameObject.SetActive(false);
            }
            else
            {
                // Incorrect :(
                Description.text = "Incorrect :(";
                
                GreenButton.gameObject.SetActive(false);

                OrangeButton.gameObject.SetActive(true);
                OrangeButton.GetComponentInChildren<Text>().text = "Retry";
                OrangeButton.onClick.RemoveAllListeners();
                OrangeButton.onClick.AddListener(ResetQuestion);
            }
        }
        
        private void ResetQuestion()
        {
            LevelSwitchStatisticsManager.Instance.LevelReset(2);
            _currentQuestionResets++;
            StartQuestion();
        }

    }

    public class ListQuestionData
    {
        public string Title { get; }
        public string Description { get; }
        public List<VehicleType> InitialState { get; }
        public AnswerChecker AnswerChecker { get; }
        public ExecuteBefore ExecuteBefore { get; }

        public ListQuestionData(string title, string description, List<VehicleType> initialState, AnswerChecker answerChecker, ExecuteBefore executeBefore)
        {
            Title = title;
            Description = description;
            InitialState = initialState;
            AnswerChecker = answerChecker;
            ExecuteBefore = executeBefore;
        }

        public ListQuestionData(string title, string description, List<VehicleType> initialState, AnswerChecker answerChecker)
        {
            Title = title;
            Description = description;
            InitialState = initialState;
            AnswerChecker = answerChecker;
        }
    }
}