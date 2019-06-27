using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ArrayOperationController : MonoBehaviour
    {
        public Button ExecuteOperationButton;
        public Dropdown IndexDropdown;
        public Dropdown IndexDropdown2 = null;
        public Text CodePreviewView;
        public ArrayOperations OperationType;
        private ArrayOperationQueue _operationsQueue;
        private int _maxIndex = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            ExecuteOperationButton.onClick.AddListener(ProcessExecution);   
        }

        public ArrayOperationQueue OperationsQueue
        {
            set => _operationsQueue = value;
        }

        public int MaxIndex
        {
            set
            {
                _maxIndex = value;
                var options = Enumerable.Range(0, _maxIndex + 1).Select(i => $"Index {i}").ToList();
                // Now populate dropdown
                IndexDropdown.AddOptions(options);
                if (IndexDropdown2 != null)
                {
                    IndexDropdown2.AddOptions(options);
                }
            }
        }

        private void ProcessExecution()
        {
            var index = IndexDropdown.value;
            int index2;
            
            switch (OperationType)
            {
                case ArrayOperations.Add:
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, index, CodePreviewView));
                    break;
                case ArrayOperations.ToTemp:
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, index, CodePreviewView));
                    break;
                case ArrayOperations.FromTemp:
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, index, CodePreviewView));
                    break;
                case ArrayOperations.Delete:
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Delete, index, CodePreviewView));
                    break;
                case ArrayOperations.CopyTo:
                    // Parse the second field
                    index2 = IndexDropdown2.value;                    
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, index, index2, CodePreviewView));
                    break;
                case ArrayOperations.Swap:
                    // Parse the second field
                    index2 = IndexDropdown2.value;          
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Swap, index, index2, CodePreviewView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ArrayOperations
    {
        Add,
        ToTemp,
        FromTemp,
        Delete,
        CopyTo,
        Reset,
        AddSpecific,
        Swap
    }
}
