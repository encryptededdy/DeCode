using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LevelManager;
using UnityEngine;

namespace DSOperationControllers
{
    public class ArrayOperationQueue : MonoBehaviour
    {
        public List<ArrayOperationController> OperationControllers;
        public ArrayLevelManager LevelManager;
        private Queue<QueuedArrayOperation> QueuedOperations = new Queue<QueuedArrayOperation>();
        private bool _queueProcessingLock = false;

        void Start()
        {
            var size = LevelManager.GetArraySize() - 1;
            OperationControllers.ForEach(obj =>
            {
                obj.OperationsQueue = this;
                obj.MaxIndex = size;
            });
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

            switch (operation.Operation)
            {
                case ArrayOperations.Add:
                    LevelManager.Spawn(obj =>
                    {
                        LevelManager.WriteToArray(obj, operation.Index1, Callback);
                    });
                    break;
                case ArrayOperations.ToTemp:
                    LevelManager.CopyFromIndexToTempVar(operation.Index1, Callback);
                    break;
                case ArrayOperations.FromTemp:
                    LevelManager.CopyFromTempVarToIndex(operation.Index1, Callback);
                    break;
                case ArrayOperations.Delete:
                    LevelManager.Destroy(operation.Index1, Callback);
                    break;
                case ArrayOperations.CopyTo:
                    LevelManager.CopyFromIndexToIndex(operation.Index1, operation.Index2, Callback);
                    break;
                case ArrayOperations.Reset:
                    LevelManager.ResetLevel(Callback);
                    break;
                case ArrayOperations.AddSpecific:
                    LevelManager.Spawn(operation.VehicleType, obj =>
                    {
                        LevelManager.WriteToArray(obj, operation.Index1, Callback);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class QueuedArrayOperation
    {
        public ArrayOperations Operation { get; }
        public int Index1 { get; }
        public int Index2 { get; }
        public VehicleType VehicleType { get; }
        
        // Used for CopyTo
        public QueuedArrayOperation(ArrayOperations operation, int index1, int index2)
        {
            Operation = operation;
            Index1 = index1;
            Index2 = index2;
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
        public QueuedArrayOperation(ArrayOperations operation, int index1)
        {
            Operation = operation;
            Index1 = index1;
        }
    }
}