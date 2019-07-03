using System;
using System.Collections.Generic;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ArrayOperationQueue : MonoBehaviour
    {
        public List<ArrayOperationController> OperationControllers;
        public ArrayLevelManager LevelManager;
        private Queue<QueuedArrayOperation> QueuedOperations = new Queue<QueuedArrayOperation>();
        private bool _queueProcessingLock = false;

        // Logging stuff
        public GameObject LoggingView;
        public Text LoggingText;
        public Button LoggingToggle;

        public QueueFinished QueueFinishedListener { get; set; }

        void Start()
        {
            var size = LevelManager.GetArraySize() - 1;
            OperationControllers.ForEach(obj =>
            {
                obj.OperationsQueue = this;
                obj.MaxIndex = size;
            });
            
            // Hide Logging stuff
            LoggingView.SetActive(false);
            LoggingToggle.onClick.AddListener(ToggleLoggingView);
        }

        public void HideAllOperations()
        {
            OperationControllers.ForEach(obj =>
            {
                obj.gameObject.SetActive(false);
            });
        }

        public void ShowOperation(ArrayOperations operation)
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
        public void UpdateSize()
        {
            var size = LevelManager.GetArraySize() - 1;
            OperationControllers.ForEach(obj =>
            {
                obj.MaxIndex = size;
            });
        }

        public List<VehicleType> GetArrayState()
        {
            return LevelManager.GetArrayState();
        }

        public void QueueOperation(QueuedArrayOperation operation)
        {
            QueuedOperations.Enqueue(operation);
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
                
            // Callback for when operation is complete
            void Callback(bool _)
            {
                print("Got callback!");
                
                // Unlock first
                _queueProcessingLock = false;
                TryExecuteQueue();
            }

            string code;

            switch (operation.Operation)
            {
                case ArrayOperations.Add:
                    LevelManager.Spawn(obj =>
                    {
                        LevelManager.WriteToArray(obj.Item2, operation.Index1, obj2 =>
                        {
                            code = $"car = {LevelManager.GetArrayState()[operation.Index1]};\n" +
                                   $"array[{operation.Index1}] = car;";
                            if (operation.CodeLine != null)
                            {
                                operation.CodeLine.text = code;
                            }
                            AppendLog(code);
                            Callback(true);
                        });
                    });
                    break;
                case ArrayOperations.ToTemp:
                    LevelManager.CopyFromIndexToTempVar(operation.Index1, Callback);
                    code = $"temp = array[{operation.Index1}];";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ArrayOperations.FromTemp:
                    LevelManager.CopyFromTempVarToIndex(operation.Index1, Callback);
                    code = $"array[{operation.Index1}] = temp;";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ArrayOperations.Delete:
                    LevelManager.Destroy(operation.Index1, Callback);
                    code = $"array[{operation.Index1}] = null;";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ArrayOperations.CopyTo:
                    LevelManager.CopyFromIndexToIndex(operation.Index1, operation.Index2, Callback);
                    code = $"array[{operation.Index2}] = array[{operation.Index1}];";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                case ArrayOperations.Reset:
                    LevelManager.ResetLevel(Callback);
                    break;
                case ArrayOperations.AddSpecific:
                    LevelManager.Spawn(obj =>
                    {
                        LevelManager.WriteToArray(obj.Item2, operation.Index1, Callback, true);
                    }, operation.VehicleType);
                    break;
                case ArrayOperations.Swap:
                    LevelManager.CopyFromIndexToTempVar(operation.Index1, obj =>
                    {
                        LevelManager.CopyFromIndexToIndex(operation.Index2, operation.Index1, obj2 =>
                        {
                            LevelManager.CopyFromTempVarToIndex(operation.Index2, Callback);
                        });
                    });
                    code = $"temp = array[{operation.Index1}];\n" +
                           $"array[{operation.Index1}] = array[{operation.Index2}];\n" +
                           $"array[{operation.Index2}] = temp;";
                    if (operation.CodeLine != null)
                    {
                        operation.CodeLine.text = code;
                    }
                    AppendLog(code);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public delegate void QueueFinished();

    public class QueuedArrayOperation
    {
        public ArrayOperations Operation { get; }
        public int Index1 { get; }
        public int Index2 { get; }
        public VehicleType VehicleType { get; }
        
        public Text CodeLine { get; }
        
        // Used for CopyTo
        public QueuedArrayOperation(ArrayOperations operation, int index1, int index2, Text codeLine)
        {
            Operation = operation;
            Index1 = index1;
            Index2 = index2;
            CodeLine = codeLine;
        }

        // Used for AddSpecific
        public QueuedArrayOperation(ArrayOperations operation, int index1, VehicleType vehicleType)
        {
            Operation = operation;
            Index1 = index1;
            VehicleType = vehicleType;
        }

        // Used for reset
        public QueuedArrayOperation(ArrayOperations operation)
        {
            Operation = operation;
        }

        // Used for all other operations
        public QueuedArrayOperation(ArrayOperations operation, int index1, Text codeLine)
        {
            Operation = operation;
            Index1 = index1;
            CodeLine = codeLine;
        }
    }
}