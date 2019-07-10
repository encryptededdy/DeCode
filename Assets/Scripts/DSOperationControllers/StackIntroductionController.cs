using System.Collections.Generic;
using CustomUI;
using LevelManager;
using UnityEngine;
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
                "Let's pushing 3 vehicles - A, B, then C. We do this by calling \"push\" on the queue",
                queue =>
                {
                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(true);
                },
                queue =>
                {
                    CameraMover.ZoomEntrance();
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Enqueue A, B then C");
                    // Enqueue
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_a, null));
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_b, null));
                    queue.QueueOperation(new QueuedStackOperation(StackOperations.PushSpecific, VehicleType.garbage_c, null));
                }));


        }

        public void Start()
        {
            SetupStackIntro();
            OperationQueue.ResetLog(); // Reset log so user doesn't have to aimlessly scroll
            NextStep();
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
                // TODO: Next Level
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