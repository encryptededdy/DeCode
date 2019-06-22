using System.Collections.Generic;
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
                    
                }));
        }

        public ArrayAlgorithmIntroductionController(ArrayOperationQueue operationQueue, Text title, Text description, Button greenButton, Button orangeButton)
        {
            _operationQueue = operationQueue;
            _title = title;
            _description = description;
            _greenButton = greenButton;
            _orangeButton = orangeButton;
            
            NextStep();
        }

        private void ReEnableGreenButton()
        {
            _greenButton.gameObject.SetActive(true);
        }

        private void NextStep()
        {
            _currentStep = _algorithmSteps.Dequeue();

            // Set buttons
            _orangeButton.gameObject.SetActive(false);
            _greenButton.gameObject.SetActive(true);
            
            _title.text = _currentStep.Title;
            _description.text = _currentStep.Description;
            _currentStep.OnDisplay(_operationQueue);
        }
    }

    public class AlgorithmStepData
    {
        internal string Title { get; }
        internal string Description { get; }
        internal OnDisplay OnDisplay { get; }

        public AlgorithmStepData(string title, string description, OnDisplay onDisplay)
        {
            Title = title;
            Description = description;
            OnDisplay = onDisplay;
        }
    }
    
    public delegate void OnDisplay(ArrayOperationQueue operationQueue);

}