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
    public class ArrayQuestionController : MonoBehaviour
    {
        public ArrayOperationQueue OperationQueue;
        private Queue<ArrayQuestionData> _questions = new Queue<ArrayQuestionData>();
        public Text Title;
        public Text Description;
        public Button GreenButton;
        public Button OrangeButton;

        public QuestionScreenLogic QuestionScreenLogic;

        private ArrayQuestionData _currentQuestion;
        private bool _runArrayAlgoIntro = false;

        private int _currentQuestionResets = 0;

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
            
            _questions.Enqueue(new ArrayQuestionData(
                "Swap Elements",
                "Swap the blue car with the red car, leaving all other cars in the same place",
                new List<VehicleType>()
                {
                    VehicleType.ambulance, VehicleType.blue, VehicleType.garbage, VehicleType.red, VehicleType.taxi
                },
                (answer) => answer.SequenceEqual(new List<VehicleType>()
                {
                    VehicleType.ambulance, VehicleType.red, VehicleType.garbage, VehicleType.blue, VehicleType.taxi
                }),
                () =>
                {
                    QuestionScreenLogic.ShowNewQuestion("Which of the below explains the purpose of these lines of code. Assume all 3 variables were defined earlier as ints.\n c = a\n a = b\n b = c",
                        "Swap the values of A and B",
                        new []{"Swap the values of A and C", "Swap the values of B and C", "Copy the value of c into a and b"},
                        "Correct - we use c as a temporary variable to store the value of a when we swap a and b.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(0, attempts));
                }));
            
            _questions.Enqueue(new ArrayQuestionData(
                "Rotate array",
                "Rotate the array to the right by 1 element (such that A is in index 0, B is in 1 etc.)",
                new List<VehicleType>()
                {
                    VehicleType.garbage_b, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e, VehicleType.garbage_a
                },
                (answer) => answer.SequenceEqual(new List<VehicleType>()
                {
                    VehicleType.garbage_a, VehicleType.garbage_b, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                }),() =>
                {
                    QuestionScreenLogic.ShowNewQuestion("Similar to before - Which of the below explains the purpose of these lines of code. Assume all 3 variables were defined earlier as ints.\n j = i\n i = k\n k = j",
                        "Swap the values of I and K",
                        new []{"Swap the values of K and J", "Swap the values of I and J", "Copy the value of J into I and K"},
                        "Correct - we use J as a temporary variable to store the value of a when we swap I and K.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(0, attempts));
                }));
                        
            OperationQueue.LevelManager.StartLevel(obj => NextQuestion());
        }

        public void AlgorithmIntroductionComplete()
        {
            // Show only swap operation
            OperationQueue.ShowOperation(ArrayOperations.Swap);
            
            // Add questions...
            _questions.Enqueue(new ArrayQuestionData(
                "First iteration",
                "Perform the first loop/iteration of the bubblesort algorithm, swapping pairs as required. If no swaps are required, just click Check",
                new List<VehicleType>()
                {
                    VehicleType.garbage_c, VehicleType.garbage_b, VehicleType.garbage_a, VehicleType.garbage_e, VehicleType.garbage_d
                },
                (answer) => answer.SequenceEqual(new List<VehicleType>()
                {
                    VehicleType.garbage_b, VehicleType.garbage_a, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                })));
            
            _questions.Enqueue(new ArrayQuestionData(
                "Second iteration",
                "Perform the second loop/iteration of the bubblesort algorithm, swapping pairs as required. If no swaps are required, just click Check",
                new List<VehicleType>()
                {
                    VehicleType.garbage_b, VehicleType.garbage_a, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                },
                (answer) => answer.SequenceEqual(new List<VehicleType>()
                {
                    VehicleType.garbage_a, VehicleType.garbage_b, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                })));
            
            _questions.Enqueue(new ArrayQuestionData(
                "Third iteration",
                "Perform the third loop/iteration of the bubblesort algorithm, swapping pairs as required. If no swaps are required, just click Check",
                new List<VehicleType>()
                {
                    VehicleType.garbage_a, VehicleType.garbage_b, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                },
                (answer) => answer.SequenceEqual(new List<VehicleType>()
                {
                    VehicleType.garbage_a, VehicleType.garbage_b, VehicleType.garbage_c, VehicleType.garbage_d, VehicleType.garbage_e
                }),
                () =>
                {
                    QuestionScreenLogic.ShowNewQuestion("Given arrays of the same length, which type of array would take the maximum number of iterations (i.e. the slowest) for BubbleSort to sort?",
                        "Reverse sorted array",
                        new []{"Already sorted array", "Sorted but the first two elements", "Array with duplicate values"},
                        "Correct - since we essentially move only one element every iteration with a reversed array, this is the slowest case.",
                        attempts => LevelSwitchStatisticsManager.Instance.QuestionReturn(1, attempts));
                }));
            
            NextQuestion();
        }

        private void NextLevel()
        {
            LevelSwitchStatisticsManager.Instance.SwitchLevel(1, 2);
        }

        private void NextQuestion()
        {
            if (_currentQuestion != null)
            {
                LevelSwitchStatisticsManager.Instance.SendSubData(1, _currentQuestionResets, _currentQuestion.Title, OperationQueue.GetLogText());
                _currentQuestionResets = 0;
            }

            switch (_questions.Count)
            {
                case 0 when !_runArrayAlgoIntro:
                {
                    // Hide all buttons...
                    OperationQueue.HideAllOperations();
                    var algoIntroCtlr = new ArrayAlgorithmIntroductionController(OperationQueue, Title, Description,
                        GreenButton, OrangeButton, this);
                    algoIntroCtlr.Start();
                    _runArrayAlgoIntro = true;
                    return;
                }
                case 0:
                    Title.text = "Arrays complete!";
                    Description.text = "We will now move on to lists, a resizable extension of arrays...";
                    GreenButton.gameObject.SetActive(true);
                    GreenButton.GetComponentInChildren<Text>().text = "Start";
                    GreenButton.onClick.RemoveAllListeners();
                    GreenButton.onClick.AddListener(NextLevel);
                    OrangeButton.gameObject.SetActive(false);
                    return;
            }
            
            _currentQuestion = _questions.Dequeue();

            if (!OperationQueue.GetArrayState().SequenceEqual(_currentQuestion.InitialState))
            {
                // Clear parking lot
                OperationQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Reset));
            }

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

        private void ResetQuestion()
        {
            LevelSwitchStatisticsManager.Instance.LevelReset(1);
            _currentQuestionResets++;
            StartQuestion();
        }

        private void CheckQuestion()
        {
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

    }

    public class ArrayQuestionData
    {
        public string Title { get; }
        public string Description { get; }
        public List<VehicleType> InitialState { get; }
        public AnswerChecker AnswerChecker { get; }
        public ExecuteBefore ExecuteBefore { get; }

        public ArrayQuestionData(string title, string description, List<VehicleType> initialState, AnswerChecker answerChecker, ExecuteBefore executeBefore)
        {
            Title = title;
            Description = description;
            InitialState = initialState;
            AnswerChecker = answerChecker;
            ExecuteBefore = executeBefore;
        }

        public ArrayQuestionData(string title, string description, List<VehicleType> initialState, AnswerChecker answerChecker)
        {
            Title = title;
            Description = description;
            InitialState = initialState;
            AnswerChecker = answerChecker;
        }
    }

    public delegate bool AnswerChecker(List<VehicleType> answer);
    public delegate void ExecuteBefore();
}