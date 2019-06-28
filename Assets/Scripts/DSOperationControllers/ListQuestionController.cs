using System.Collections.Generic;
using System.Linq;
using LevelManager;
using UnityEngine;
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

        private ListQuestionData _currentQuestion;
        private bool _runArrayAlgoIntro = false;

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
                }
            ));
            
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add Again^2",
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
                }
            ));
            
            _questions.Enqueue(new ListQuestionData(
                "Expand and Add Again",
                "Now, add another 3 vehicles. We can see that the doubling method is much more efficient, as it avoids overusing the expensive copying operation.",
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
                    return correct && answer.Count == 5;
                }
            ));
                        
            NextQuestion();
            
        }

        private void NextLevel()
        {
            // TODO: Load the queue level...
        }

        private void NextQuestion()
        {
            if (_questions.Count == 0)
            {
                Title.text = "Lists complete!";
                Description.text = "Lorem Ipsum...";
                GreenButton.gameObject.SetActive(true);
                GreenButton.GetComponentInChildren<Text>().text = "Start";
                GreenButton.onClick.RemoveAllListeners();
                GreenButton.onClick.AddListener(NextLevel);
                OrangeButton.gameObject.SetActive(false);
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
            OrangeButton.onClick.AddListener(StartQuestion);
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
                OrangeButton.onClick.AddListener(StartQuestion);
            }
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

    public delegate void ExecuteBefore();
}