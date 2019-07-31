using System.Collections.Generic;
using CustomUI;
using LevelManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class StackIntroductionController: MonoBehaviour
    {
        public StackOperationQueue OperationQueue;
        public Text Title;
        public Text Description;
        public Button GreenButton;
        public Button OrangeButton;

        public SmoothCameraMover CameraMover;

        public QuestionScreenLogic QuestionScreenLogic;
        private readonly Queue<StackStepData> _algorithmSteps = new Queue<StackStepData>();

        public GameObject ManualControls;
        
        private StackStepData _currentStep;
        
        private void SetupStackIntro()
        {
            _algorithmSteps.Enqueue(new StackStepData("Pushing",
                "Let's pushing 3 vehicles - A, B, then C. We do this by calling \"push\" on the stack",
                queue =>
                {
                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(true);
                    CameraMover.ZoomEntranceStack();
                },
                queue =>
                {
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Push A, B then C");
                    // Enqueue
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_a, null));
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_b, null));
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_c, null));
                }));
            _algorithmSteps.Enqueue(new StackStepData("Popping",
                "We can now pop - we can see that X is popped since it was the last vehicle added (remember - last in, first out.)",
                queue =>
                {
                    QuestionScreenLogic.ShowNewQuestion("We pushed A, then B, then C just before. Now if we pop, what element do we get back? Recall that a Stack is FILO (First-in, Last-out)",
                        "C",
                        new []{"A", "B", "None"},
                        "",
                        attempts =>
                        {
                            Description.text =
                                "We can now pop - we can see that C is popped since it was the last vehicle added (remember - last in, first out.)";
                            print($"{attempts} attempts until correct");
                        });

                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(true);
                },
                queue =>
                {
                    CameraMover.ZoomEntranceStack();
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Dequeue returns A");
                    // Enqueue
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.Pop, null));
                }));
                        
            _algorithmSteps.Enqueue(new StackStepData("Try yourself",
                "Now let's take a look at how an array-based implementation of a stack could work, using an array. Try pushing a vehicle, then popping one, and pay attention to what happens.",
                queue =>
                {
                    CameraMover.ZoomNormal();
                    queue.SetHidden(true);
                    GreenButton.gameObject.SetActive(true);
                    ManualControls.SetActive(true);
                    OrangeButton.gameObject.SetActive(false);
                },
                queue =>
                {
                    // Nothing.
                }));
            
            _algorithmSteps.Enqueue(new StackStepData("Try yourself",
                "Now let's take a look at how an array-based implementation of a stack could work, using an array. Try pushing a vehicle, then popping one, and pay attention to what happens.",
                queue =>
                {
                    QuestionScreenLogic.ShowNewQuestion("Now let's consider performance. If we assume the size of the stack never exceeds the size of the array (i.e. we never have to expand the array), can we make this stack implementation any faster?",
                        "No",
                        new []{"Yes, Pop can be faster", "Yes, Push can be faster", "Yes, Push & Pop can be faster"},
                        "No, since both push and pop only requiring writing/reading one element from the internal array. There are no additional operations to eliminate in order to speed it up.",
                        attempts =>
                        {
                            print($"{attempts} attempts until correct");
                        });

                    CameraMover.ZoomNormal();
                    queue.SetHidden(true);
                    GreenButton.gameObject.SetActive(true);
                    ManualControls.SetActive(true);
                    OrangeButton.gameObject.SetActive(false);
                },
                queue =>
                {
                    // Nothing.
                }));

        }

        public void Start()
        {
            SetupStackIntro();
            OperationQueue.ResetLog(); // Reset log so user doesn't have to aimlessly scroll
            OperationQueue.LevelManager.StartLevel(obj => NextStep());
        }
        
        private void ReEnableGreenButton()
        {
            GreenButton.gameObject.SetActive(true);
        }
        
        private void ReEnableOrangeButton()
        {
            OrangeButton.gameObject.SetActive(true);
        }

        private void OnOrangeClick()
        {
            _currentStep.OnOrangeClick?.Invoke(OperationQueue);
        }

        private void NextStep()
        {
            if (_algorithmSteps.Count == 0)
            {
                GreenButton.onClick.RemoveAllListeners();
                OrangeButton.onClick.RemoveAllListeners();
                SceneManager.LoadScene("Stats");
                return;
            }

            GreenButton.gameObject.SetActive(false);
            OrangeButton.gameObject.SetActive(false);
            
            GreenButton.onClick.RemoveAllListeners();
            OrangeButton.onClick.RemoveAllListeners();
            
            GreenButton.onClick.AddListener(NextStep);
            OrangeButton.onClick.AddListener(OnOrangeClick);
            
            _currentStep = _algorithmSteps.Dequeue();
            
            GreenButton.GetComponentInChildren<Text>().text = "Next";
            OrangeButton.GetComponentInChildren<Text>().text = "Execute";
            
            Title.text = _currentStep.Title;
            Description.text = _currentStep.Description;
            _currentStep.OnDisplay?.Invoke(OperationQueue);
        }
    }

    public class StackStepData
    {
        internal string Title { get; }
        internal string Description { get; }
        internal QueueStackOperation OnDisplay { get; }
        internal QueueStackOperation OnOrangeClick { get; }

        public StackStepData(string title, string description, QueueStackOperation onDisplay, QueueStackOperation onOrangeClick = null)
        {
            Title = title;
            Description = description;
            OnDisplay = onDisplay;
            OnOrangeClick = onOrangeClick;
        }
    }
    
    public delegate void QueueStackOperation(StackOperationQueue operationQueue);

}