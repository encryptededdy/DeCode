using System;
using System.Collections.Generic;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ListOperationQueue : MonoBehaviour
    {
        public List<ListOperationController> OperationControllers;
        public ListLevelManager LevelManager;
        private Queue<QueuedListOperation> QueuedOperations = new Queue<QueuedListOperation>();
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

            _queueProcessingLock = true;
            LevelManager.CreateNewCarpark(1, obj =>
            {
                var size = LevelManager.GetArraySize() - 1;
                print($"Size is {LevelManager.GetArraySize()}");
                OperationControllers.ForEach(obj2 =>
                {
                    obj2.OperationsQueue = this;
                    obj2.MaxIndex = size;
                });
                Callback(true);
            });
        }

        public void HideAllOperations()
        {
            OperationControllers.ForEach(obj =>
            {
                obj.gameObject.SetActive(false);
            });
        }

        public void ShowOperation(ListOperations operation)
        {
            OperationControllers.ForEach(obj =>
            {
                if (obj.OperationType.Equals(operation))
                {
                    obj.gameObject.SetActive(true);
                }
            });
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

        // Call if the CarPark size changes
        public void UpdateSize(int size)
        {
            print($"Update size to {size}");
            // var size = LevelManager.GetArraySize() - 1;
            OperationControllers.ForEach(obj =>
            {
                obj.MaxIndex = size - 1;
            });
        }

        public List<VehicleType> GetArrayState()
        {
            return LevelManager.GetArrayState();
        }

        public void QueueOperation(QueuedListOperation operation)
        {
            QueuedOperations.Enqueue(operation);
            TryExecuteQueue();
        }
        
        // Callback for when operation is complete
        void Callback(bool _)
        {
            print("Got callback!");
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
                case ListOperations.Add:
                    LevelManager.Spawn(obj =>
                    {
                        print($"Writing to array at {operation.Index1}");
                        LevelManager.WriteToArray(obj.Item2, operation.Index1, obj2 =>
                        {
                            code = $"list[{operation.Index1}] = {LevelManager.GetArrayState()[operation.Index1].ToCapitalizedString()};";
                            if (operation.CodeLine != null)
                            {
                                operation.CodeLine.text = code;
                            }
                            AppendLog(code);
                            Callback(true);
                        });
                    });
                    break;
                case ListOperations.CopyOne:
                    LevelManager.CreateNewCarpark(LevelManager.GetArraySize() + 1, Callback);
                    UpdateSize(LevelManager.GetArraySize() + 1);
                    code = $"// Expand internal array to {LevelManager.GetArraySize() + 1}";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ListOperations.CopyDouble:
                    LevelManager.CreateNewCarpark(LevelManager.GetArraySize()*2, Callback);
                    UpdateSize(LevelManager.GetArraySize() * 2);
                    code = $"// Expand internal array to {LevelManager.GetArraySize() * 2}";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ListOperations.Delete:
                    LevelManager.Destroy(operation.Index1, Callback);
                    code = $"list[{operation.Index1}] = null;";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ListOperations.Reset:
                    LevelManager.ResetLevel(Callback);
                    UpdateSize(LevelManager.GetArraySize());
                    break;
                case ListOperations.SetSize:
                    LevelManager.CreateNewCarpark(operation.Index1, Callback);
                    UpdateSize(operation.Index1);
                    break;
                case ListOperations.AddSpecific:
                    throw new ArgumentOutOfRangeException($"Unimplemented for Lists");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
    
    public class QueuedListOperation
    {
        public ListOperations Operation { get; }
        public int Index1 { get; }
        public Text CodeLine { get; }
        
        // Used for expand operations
        public QueuedListOperation(ListOperations operation, Text codeLine)
        {
            Operation = operation;
            CodeLine = codeLine;
        }

        // Used for all other operations
        public QueuedListOperation(ListOperations operation, int index1, Text codeLine)
        {
            Operation = operation;
            Index1 = index1;
            CodeLine = codeLine;
        }
    }
}