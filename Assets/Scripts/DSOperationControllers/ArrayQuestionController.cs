using System.Collections.Generic;
using System.Linq;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ArrayQuestionController : MonoBehaviour
    {
        public ArrayOperationQueue OperationQueue;
        private Queue<ArrayQuestionData> _questions = new Queue<ArrayQuestionData>();
        public Text Title;
        public Text Description;
        public Button GreenButton;
        public Button OrangeButton;

        private ArrayQuestionData _currentQuestion;
        private bool _runArrayAlgoIntro = false;

        private void Start()
        {
            // Define questions...
            _questions.Enqueue(new ArrayQuestionData(
                "Fill Array",
                "Fully populate the array with whatever vehicles you like!",
                new List<VehicleType>()
                {
                    VehicleType.empty, VehicleType.empty, VehicleType.empty, VehicleType.empty, VehicleType.empty
                },
                (answer) =>
                {
                    var correct = true;
                    foreach (var obj in answer)
                    {
                        if (obj.Equals(VehicleType.empty)) correct = false;
                    }
                    return correct;
                }
             ));
            
            // TODO: Removed for testing only...
//            _questions.Enqueue(new ArrayQuestionData(
//                "Swap Elements",
//                "Swap the blue car with the red car, leaving all other cars in the same place",
//                new List<VehicleType>()
//                {
//                    VehicleType.ambulance, VehicleType.blue, VehicleType.garbage, VehicleType.red, VehicleType.taxi
//                },
//                (answer) => answer.SequenceEqual(new List<VehicleType>()
//                {
//                    VehicleType.ambulance, VehicleType.red, VehicleType.garbage, VehicleType.blue, VehicleType.taxi
//                })));
//            
//            _questions.Enqueue(new ArrayQuestionData(
//                "Sort Array",
//                "Sort the array according to car type, ascending - Ambulance, Police, Taxi, Silver, Black.",
//                new List<VehicleType>()
//                {
//                    VehicleType.police, VehicleType.ambulance, VehicleType.black, VehicleType.taxi, VehicleType.silver
//                },
//                (answer) => answer.SequenceEqual(new List<VehicleType>()
//                {
//                    VehicleType.ambulance, VehicleType.police, VehicleType.taxi, VehicleType.silver, VehicleType.black
//                })));
                        
            NextQuestion();
            
        }

        public void AlgorithmIntroductionComplete()
        {
            NextQuestion();
        }

        private void NextQuestion()
        {
            // Clear parking lot
            OperationQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Reset));

            if (_questions.Count == 0 && !_runArrayAlgoIntro)
            {
                var algoIntroCtlr = new ArrayAlgorithmIntroductionController(OperationQueue, Title, Description,
                    GreenButton, OrangeButton, this);
                algoIntroCtlr.Start();
                _runArrayAlgoIntro = true;
                return;
            }
            _currentQuestion = _questions.Dequeue();
            // Fill in UI
            Title.text = _currentQuestion.Title;
            Description.text = "Click Start to begin question";
            // Configure buttons
            GreenButton.gameObject.SetActive(true);
            GreenButton.GetComponentInChildren<Text>().text = "Start";
            GreenButton.onClick.RemoveAllListeners();
            GreenButton.onClick.AddListener(StartQuestion);
            
            OrangeButton.gameObject.SetActive(false);
        }

        private void StartQuestion()
        {
            // Fill in UI
            Title.text = _currentQuestion.Title;
            Description.text = _currentQuestion.Description;

            // Clear parking lot
            OperationQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Reset));
            // Add cars
            for (var i = 0; i < _currentQuestion.InitialState.Count; i++)
            {
                // Work around issue with trying to insert VehicleType.empty
                if (!_currentQuestion.InitialState[i].Equals(VehicleType.empty))
                {
                    OperationQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.AddSpecific, i,
                        _currentQuestion.InitialState[i]));
                }
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
            print(OperationQueue.GetArrayState());
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

    public class ArrayQuestionData
    {
        public string Title { get; }
        public string Description { get; }
        public List<VehicleType> InitialState { get; }
        public AnswerChecker AnswerChecker { get; }

        public ArrayQuestionData(string title, string description, List<VehicleType> initialState, AnswerChecker answerChecker)
        {
            Title = title;
            Description = description;
            InitialState = initialState;
            AnswerChecker = answerChecker;
        }
    }

    public delegate bool AnswerChecker(List<VehicleType> answer);
}