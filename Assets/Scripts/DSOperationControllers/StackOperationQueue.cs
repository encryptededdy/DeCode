using System;
using System.Collections.Generic;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class StackOperationQueue : MonoBehaviour
    {
        public StackLevelManager LevelManager;
        private Queue<QueuedStackOperation> QueuedOperations = new Queue<QueuedStackOperation>();
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
        
        public void QueueOperation(QueuedStackOperation operation)
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
                case StackOperations.Push:
                    LevelManager.Spawn(obj =>
                    {
                        if (obj == null)
                        {
                            if (operation.CodeLine != null)
                            {
                                operation.CodeLine.text = "Stack is full!";
                            }
                            Callback(true);
                            return;
                        }
                        code = $"queue.push({obj.Item1.ToCapitalizedString()});";
                        if (operation.CodeLine != null)
                        {
                            operation.CodeLine.text = code;
                        }
                        AppendLog(code);
                        LevelManager.Push(obj.Item2, obj2 =>
                        {
                            Callback(true);
                        });
                    });
                    break;
                case StackOperations.Reset:
                    LevelManager.ResetLevel(Callback);
                    break;
                case StackOperations.PushSpecific:
                    LevelManager.Spawn(obj =>
                    {
                        code = $"car = {operation.Type};\n" +
                               $"queue.push(car);";
                        if (operation.CodeLine != null)
                        {
                            operation.CodeLine.text = code;
                        }
                        AppendLog(code);
                        LevelManager.Push(obj.Item2, obj2 =>
                        {
                            Callback(true);
                        });
                    }, operation.Type);
                    break;
                case StackOperations.Pop:
                    code = "queue.pop();";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);

                    LevelManager.Pop(Callback); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
    
    public class QueuedStackOperation
    {
        public QueuedStackOperation(StackOperations operation, VehicleType type, Text codeLine = null)
        {
            Operation = operation;
            Type = type;
            CodeLine = codeLine;
        }

        public QueuedStackOperation(StackOperations operation, Text codeLine = null)
        {
            Operation = operation;
            CodeLine = codeLine;
        }

        public StackOperations Operation { get; }
        public VehicleType Type { get; }
        public Text CodeLine { get; }
    }

    public enum StackOperations
    {
        Push,
        PushSpecific,
        Pop,
        Reset
    }
}