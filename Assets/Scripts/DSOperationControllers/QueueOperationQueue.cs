using System;
using System.Collections.Generic;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class QueueOperationQueue : MonoBehaviour
    {
        public QueueLevelManager LevelManager;
        private Queue<QueuedQueueOperation> QueuedOperations = new Queue<QueuedQueueOperation>();
        private bool _queueProcessingLock = false;

        // Logging stuff
        public GameObject LoggingView;
        public Text LoggingText;
        public Button LoggingToggle;

        public QueueFinished QueueFinishedListener { get; set; }

        void Start()
        {
            // Hide Logging stuff
            LoggingView.SetActive(false);
            LoggingToggle.onClick.AddListener(ToggleLoggingView);
        }

        private void ToggleLoggingView()
        {
            LoggingView.SetActive(!LoggingView.activeInHierarchy);
        }

        public void ResetLog()
        {
            LoggingText.text = "";
        }

        public void AppendLog(String line)
        {
            LoggingText.text = LoggingText.text + "\n" + line;
        }

        public void SetHidden(bool hidden)
        {
            LevelManager.SetHiddenImplementation(hidden);
        }

        public void MakeCircular()
        {
            LevelManager.SetCircularQueue(true); // Conversion from circular to non-circular is unsupported.
        }
        
        public void QueueOperation(QueuedQueueOperation operation)
        {
            QueuedOperations.Enqueue(operation);
            TryExecuteQueue();
        }
        
        // Callback for when operation is complete
        void Callback(bool _)
        {
            print("Got Callback");
            // Unlock first
            _queueProcessingLock = false;
            TryExecuteQueue();
        }

        private void TryExecuteQueue()
        {
            /**
             * We don't need a "real" lock/semaphore since this is all on the same thread. The lock is more to indicate
             * that we're waiting for a callback.
             */
            if (_queueProcessingLock) return;
            _queueProcessingLock = true;
            
            if (QueuedOperations.Count == 0)
            {
                _queueProcessingLock = false;
                QueueFinishedListener?.Invoke();
                return;
            }
            
            // Have lock, try to do stuff
            var operation = QueuedOperations.Dequeue();

            string code;

            switch (operation.Operation)
            {
                case QueueOperations.Enqueue:
                    LevelManager.Spawn(obj =>
                    {
                        if (obj == null)
                        {
                            if (operation.CodeLine != null)
                            {
                                operation.CodeLine.text = "Queue is full!";
                            }
                            Callback(true);
                            return;
                        }
                        code = $"car = {obj.Item1};\n" +
                               $"queue.enqueue(car);";
                        if (operation.CodeLine != null)
                        {
                            operation.CodeLine.text = code;
                        }
                        AppendLog(code);
                        LevelManager.Enqueue(obj.Item2, obj2 =>
                        {
                            Callback(true);
                        });
                    });
                    break;
                case QueueOperations.Reset:
                    LevelManager.ResetLevel(Callback);
                    break;
                case QueueOperations.EnqueueSpecific:
                    LevelManager.Spawn(obj =>
                    {
                        code = $"car = {operation.Type};\n" +
                               $"queue.enqueue(car);";
                        if (operation.CodeLine != null)
                        {
                            operation.CodeLine.text = code;
                        }
                        AppendLog(code);
                        LevelManager.Enqueue(obj.Item2, obj2 =>
                        {
                            Callback(true);
                        });
                    }, operation.Type);
                    break;
                case QueueOperations.Dequeue:
                    code = "queue.dequeue();";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);

                    LevelManager.Dequeue(Callback); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
    
    public class QueuedQueueOperation
    {
        public QueuedQueueOperation(QueueOperations operation, VehicleType type, Text codeLine = null)
        {
            Operation = operation;
            Type = type;
            CodeLine = codeLine;
        }

        public QueuedQueueOperation(QueueOperations operation, Text codeLine = null)
        {
            Operation = operation;
            CodeLine = codeLine;
        }

        public QueueOperations Operation { get; }
        public VehicleType Type { get; }
        public Text CodeLine { get; }
    }

    public enum QueueOperations
    {
        Enqueue,
        EnqueueSpecific,
        Dequeue,
        Reset
    }
}