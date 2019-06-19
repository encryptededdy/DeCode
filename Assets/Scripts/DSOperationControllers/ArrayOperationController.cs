using System;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ArrayOperationController : MonoBehaviour
    {
        public Button ExecuteOperationButton;
        public InputField IndexInputField;
        public InputField IndexInputField2 = null;
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
            set { _operationsQueue = value; }
        }

        public int MaxIndex
        {
            set { _maxIndex = value; }
        }

        private void ProcessExecution()
        {
            if (IndexInputField.text.Length == 0)
            {
                CodePreviewView.text = "Invalid Index";
                return;
            }

            var index = int.Parse(IndexInputField.text);

            if (index > _maxIndex)
            {
                CodePreviewView.text = "Index out of range";
                return;
            }
            
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
                    if (IndexInputField2.text.Length == 0)
                    {
                        CodePreviewView.text = "Invalid Index";
                        return;
                    }

                    var index2 = int.Parse(IndexInputField2.text);

                    if (index2 > _maxIndex)
                    {
                        CodePreviewView.text = "Index out of range";
                        return;
                    }
                    
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
        CopyTo
    }
}
