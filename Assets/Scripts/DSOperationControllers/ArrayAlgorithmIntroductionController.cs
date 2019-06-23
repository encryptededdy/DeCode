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

        private void SetupStepsBubbleSort()
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
            
            // A, [B, E], D, C

            _algorithmSteps.Enqueue(new AlgorithmStepData("Next pair",
                "When we compare cars at index 1 and 2, we see that they're in the right order (e comes after a).",
                queue => { _greenButton.gameObject.SetActive(true); }
            ));
            
            // A, B, [E, D], C

            _algorithmSteps.Enqueue(new AlgorithmStepData("Swap pairs",
                "We then compare the next pairs, 2 and 3. Since E (index 2) is after D (index 3), we swap them too.",
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
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, 3, "// Swap cars at index 2 and 3\ntemp = array[3]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, 2, 3, "array[3] = array[2]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, 2, "array[2] = temp"));
                    
                }));

            // A, B, D, [E, C]
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Swap again",
                "You should get the hang of this now. Upon checking the last two pairs, we can see that C should go before E so we swap them too.",
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
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, 3, "// Swap cars at index 3 and 4\ntemp = array[4]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, 2, 3, "array[4] = array[3]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, 2, "array[3] = temp"));
                    
                }));
            
            // A, B, D, C, E
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Iteration 2",
                "For the second iteration, we do the same thing again - check every pair, swap those that are out of order...",
                queue =>
                {
                    queue.AppendLog("// Iteration 2");
                    _greenButton.gameObject.SetActive(true);
                }
            ));
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Iteration 2",
                "...(0, 1), (1, 2) are in order, however (2, 3) are out of order - C should come before D. Thus, we swap them.",
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
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, 3, "// Swap cars at index 2 and 3\ntemp = array[3]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, 2, 3, "array[3] = array[2]"));
                    queue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, 2, "array[2] = temp"));                    
                }
            ));
            
            // A, B, C, D, E
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Iteration 2",
                "Finally, we check (3, 4) which are now in order. We therefore continue to Iteration 3...",
                queue =>
                {
                    _greenButton.gameObject.SetActive(true);
                }
            ));
            
            _algorithmSteps.Enqueue(new AlgorithmStepData("Iteration 3",
                "For the final iteration, we check all pairs and find that they are all in order. The algorithm then terminates and the array is sorted.",
                queue =>
                {
                    queue.AppendLog("// Iteration 3 results in no swaps");
                    _greenButton.gameObject.SetActive(true);
                }
            ));



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
            if (_algorithmSteps.Count == 0)
            {
                // TODO: End code
                return;
            }

            _greenButton.gameObject.SetActive(false);
            _orangeButton.gameObject.SetActive(false);
            
            _greenButton.onClick.AddListener(NextStep);
            _orangeButton.onClick.AddListener(OnOrangeClick);
            
            _currentStep = _algorithmSteps.Dequeue();
            
            _greenButton.GetComponentInChildren<Text>().text = "Next";
            _orangeButton.GetComponentInChildren<Text>().text = "Execute";
            
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