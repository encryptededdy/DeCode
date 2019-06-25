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
            string code;
            
            switch (OperationType)
            {
                case ArrayOperations.Add:
                    code = "car = new Car();\n" +
                                    $"array[{index}] = car;";
                    CodePreviewView.text = code;
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Add, index, code));
                    break;
                case ArrayOperations.ToTemp:
                    code = $"temp = array[{index}];";
                    CodePreviewView.text = code;
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.ToTemp, index, code));
                    break;
                case ArrayOperations.FromTemp:
                    code = $"array[{index}] = temp;";
                    CodePreviewView.text = code;
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.FromTemp, index, code));
                    break;
                case ArrayOperations.Delete:
                    code = $"array[{index}] = null;";
                    CodePreviewView.text = code;
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Delete, index, code));
                    break;
                case ArrayOperations.CopyTo:
                    // Parse the second field
                    index2 = IndexDropdown2.value;                    
                    code = $"array[{index2}] = array[{index}];";
                    CodePreviewView.text = code;
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.CopyTo, index, index2, code));
                    break;
                case ArrayOperations.Swap:
                    // Parse the second field
                    index2 = IndexDropdown2.value;          
                    code = $"temp = array[{index}];\n" +
                           $"array[{index}] = array[{index2}];\n" +
                           $"array[{index2}] = temp;";
                    _operationsQueue.QueueOperation(new QueuedArrayOperation(ArrayOperations.Swap, index, index2, code));
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
