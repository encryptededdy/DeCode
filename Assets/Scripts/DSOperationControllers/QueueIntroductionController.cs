using System.Collections.Generic;
using CustomUI;
using LevelManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class QueueIntroductionController: MonoBehaviour
    {
        public QueueOperationQueue OperationQueue;
        public Text Title;
        public Text Description;
        public Button GreenButton;
        public Button OrangeButton;

        public SmoothCameraMover CameraMover;

        public QuestionScreenLogic QuestionScreenLogic;
        private readonly Queue<QueueStepData> _algorithmSteps = new Queue<QueueStepData>();

        public GameObject ManualControls;
        
        private QueueStepData _currentStep;
        
        private void SetupQueueIntro()
        {
            _algorithmSteps.Enqueue(new QueueStepData("Enqueuing",
                "Let's add 3 vehicles - A, B, then C. We do this by calling \"enqueue\" on the queue",
                queue =>
                {
                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(true);
                    CameraMover.ZoomEntrance();
                },
                queue =>
                {
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Enqueue A, B then C");
                    // Enqueue
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_a, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_b, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_c, null));
                }));
            
            _algorithmSteps.Enqueue(new QueueStepData("Dequeuing",
                "We can now dequeue - we can see that A is dequeued since it was the first vehicle added (remember - first in, first out.)",
                queue =>
                {
                    QuestionScreenLogic.ShowNewQuestion("We enqueued A, then B, then C just before. Now if we dequeue, what element do we get back? Recall that a Queue is FIFO (First-in, First-out)",
                        "A",
                        new []{"B", "C", "None"},
                        attempts => print($"{attempts} attempts until correct"));

                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(true);
                },
                queue =>
                {
                    CameraMover.ZoomExit();
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Dequeue returns A");
                    // Enqueue
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Dequeue, null));
                }));
            
            _algorithmSteps.Enqueue(new QueueStepData("Naive Implementation",
                "Now let's take a look at how a naive implementation of a queue could work, using an array. Try enqueuing a vehicle, then dequeuing one, and pay attention to what happens.",
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
            
            _algorithmSteps.Enqueue(new QueueStepData("Naive Implementation",
                "", // Hidden as it reveals the answer!
                queue =>
                {
                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Reset));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_a, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_b, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_c, null));
                    queue.QueueFinishedListener = () => OrangeButton.gameObject.SetActive(true);
                    QuestionScreenLogic.ShowNewQuestion("What is a possible disadvantage of this naive implementation of a queue using an array that is copied every dequeue. Assume that the Queue is not expected to expand in size beyond 7.",
                        "Dequeue operations are slow",
                        new []{"Memory is wasted", "Enqueue operations are slow", "This is the most efficient implementation"},
                        attempts =>
                        {
                            print($"{attempts} attempts until correct");
                            Description.text =
                                "Let's enqueue a vehicle, then dequeue one while observing performance. It's obvious to us that while enqueuing is fast, dequeuing is slow since the array copy has to occur.";
                        });

                },
                queue =>
                {
                    CameraMover.ZoomNormalMagnified();
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Enqueue D, then dequeue.");
                    // Enqueue
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_d, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Dequeue, null));
                }));
                
            _algorithmSteps.Enqueue(new QueueStepData("Improved Implementation",
                "Let's now consider a improved implementation where we instead use a pointer (the red flag) to define the end of the queue. Try enqueuing and dequeuing vehicles and observe the performance.",
                queue =>
                {
                    queue.MakeCircular();                    
                    GreenButton.gameObject.SetActive(true);
                    ManualControls.SetActive(true);
                    OrangeButton.gameObject.SetActive(false);
                    QuestionScreenLogic.ShowNewQuestion("If we modify our implementation such that we can define the end of the queue arbitrarily at any index, would this result in improved performance when compared to the naive implementation shown beforehand?",
                        "Yes, Dequeue would be sped up",
                        new []{"Yes, Enqueue would be sped up", "Yes, De/Enqueue would both be sped up", "No, it would slow down"},
                        attempts => print($"{attempts} attempts until correct"));
                },
                queue =>
                {
                    // Nothing.
                }));
            
            _algorithmSteps.Enqueue(new QueueStepData("Improved Implementation",
                "", // Hidden as it reveals the answer!
                queue =>
                {
                    GreenButton.gameObject.SetActive(false);
                    ManualControls.SetActive(false);
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Reset));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_a, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_b, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_c, null));
                    queue.QueueFinishedListener = () => OrangeButton.gameObject.SetActive(true);
                    QuestionScreenLogic.ShowNewQuestion("Why are dequeues faster with this new, improved implementation? Consider what had to happen when dequeues were done on the old, naive implementation.",
                        "No array copies are needed",
                        new []{"We use less memory", "The copy operation is faster", "Dequeues are not faster"},
                        attempts =>
                        {
                            print($"{attempts} attempts until correct");
                            Description.text =
                                "Let's enqueue a vehicle, then dequeue one while observing performance. We can see that dequeuing is just as fast as enqueuing now, as we simply need to move the flag instead of performing the expensive copying operation";
                        });

                },
                queue =>
                {
                    OrangeButton.gameObject.SetActive(false);
                    queue.QueueFinishedListener = ReEnableGreenButton;
                    queue.AppendLog("// Enqueue D, then dequeue.");
                    // Enqueue
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.EnqueueSpecific, VehicleType.garbage_d, null));
                    queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Dequeue, null));
                }));


        }

        public void Start()
        {
            SetupQueueIntro();
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
                SceneManager.LoadScene("Stack");
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

    public class QueueStepData
    {
        internal string Title { get; }
        internal string Description { get; }
        internal QueueQueueOperation OnDisplay { get; }
        internal QueueQueueOperation OnOrangeClick { get; }

        public QueueStepData(string title, string description, QueueQueueOperation onDisplay, QueueQueueOperation onOrangeClick = null)
        {
            Title = title;
            Description = description;
            OnDisplay = onDisplay;
            OnOrangeClick = onOrangeClick;
        }
    }
    
    public delegate void QueueQueueOperation(QueueOperationQueue operationQueue);

}