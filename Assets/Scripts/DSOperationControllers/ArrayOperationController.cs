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
            
            switch (OperationType)
            {
                case ArrayOperations.Add:
                    CodePreviewView.text = "car = new Car();\n" +
                                           $"array[{index}] = car;";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, index));
                    break;
                case ArrayOperations.ToTemp:
                    CodePreviewView.text = $"temp = array[{index}];";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, index));
                    break;
                case ArrayOperations.FromTemp:
                    CodePreviewView.text = $"array[{index}] = temp;";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, index));
                    break;
                case ArrayOperations.Delete:
                    CodePreviewView.text = $"array[{index}] = null;";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Delete, index));
                    break;
                case ArrayOperations.CopyTo:
                    // Parse the second field
                    var index2 = IndexDropdown2.value;                    
                    CodePreviewView.text = $"array[{index2}] = array[{index}];";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, index, index2));
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
        AddSpecific
    }
}
