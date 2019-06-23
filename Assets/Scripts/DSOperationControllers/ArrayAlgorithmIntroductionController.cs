using System.Collections.Generic;
using LevelManager;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ArrayAlgorithmIntroductionController
    {
        private ArrayOperationQueue _operationQueue;
        private readonly Text _title;
        private readonly Text _description;
        private readonly Button _greenButton;
        private readonly Button _orangeButton;

        private Queue<AlgorithmStepData> _algorithmSteps = new Queue<AlgorithmStepData>();

        private AlgorithmStepData _currentStep;

        public void SetupStepsBubbleSort()
        {
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("BubbleSort",
                "Bubblesort is a simple sorting algorithm that repeatedly steps through the list, compares adjacent pairs and swaps them if they are in the wrong order.",
                queue =>
                {
                    _greenButton.gameObject.SetActive(false);
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, 0, VehicleType.garbage_b));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, 1, VehicleType.garbage_a));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, 0, VehicleType.garbage_e));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, 0, VehicleType.garbage_d));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, 0, VehicleType.garbage_c));
                    queue.QueueFinishedListener = ReEnableGreenButton;
                }));
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Step 1",
                "We start by comparing the first two pairs. Since B (index 0) is after A (index 1), we swap them.",
                queue =>
                {
                    _greenButton.gameObject.SetActive(false);
                    _orangeButton.gameObject.SetActive(true);
                },
                queue =>
                {
                    _orangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    // Perform Swap
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, 1, "// Swap cars at index 0 and 1\ntemp = array[1]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, 0, 1, "array[1] = array[0]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, 0, "array[0] = temp"));
                    
                }));
        }

        public ArrayAlgorithmIntroductionController(ArrayOperationQueue operationQueue, Text title, Text description, Button greenButton, Button orangeButton)
        {
            _operationQueue = operationQueue;
            _title = title;
            _description = description;
            _greenButton = greenButton;
            _orangeButton = orangeButton;
            
            SetupStepsBubbleSort();
            
            operationQueue.ResetLog(); // Reset log so user doesn't have to aimlessly scroll
            NextStep();
        }

        private void ReEnableGreenButton()
        {
            _greenButton.gameObject.SetActive(true);
        }
        
        private void ReEnableOrangeButton()
        {
            _orangeButton.gameObject.SetActive(true);
        }

        private void OnOrangeClick()
        {
            _currentStep.OnOrangeClick?.Invoke(_operationQueue);
        }

        private void NextStep()
        {
            _greenButton.gameObject.SetActive(false);
            _orangeButton.gameObject.SetActive(false);
            
            _greenButton.onClick.AddListener(NextStep);
            _orangeButton.onClick.AddListener(OnOrangeClick);
            
            _currentStep = _algorithmSteps.Dequeue();
            
            _greenButton.GetComponentInChildren<Text>().text = "Next";
            _orangeButton.GetComponentInChildren<Text>().text = "Run";
            
            _title.text = _currentStep.Title;
            _description.text = _currentStep.Description;
            _currentStep.OnDisplay?.Invoke(_operationQueue);
        }
    }

    public class AlgorithmStepData
    {
        internal string Title { get; }
        internal string Description { get; }
        internal QueueOperation OnDisplay { get; }
        internal QueueOperation OnOrangeClick { get; }

        public AlgorithmStepData(string title, string description, QueueOperation onDisplay, QueueOperation onOrangeClick = null)
        {
            Title = title;
            Description = description;
            OnDisplay = onDisplay;
            OnOrangeClick = onOrangeClick;
        }
    }
    
    public delegate void QueueOperation(ArrayOperationQueue operationQueue);

}