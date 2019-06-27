using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class ListOperationController : MonoBehaviour
    {
        public Button ExecuteOperationButton;
        public Dropdown IndexDropdown;
        public Text CodePreviewView;
        public ListOperations OperationType;
        private ListOperationQueue _operationsQueue;
        private int _maxIndex = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            ExecuteOperationButton.onClick.AddListener(ProcessExecution);   
        }

        public ListOperationQueue OperationsQueue
        {
            set => _operationsQueue = value;
        }

        public int MaxIndex
        {
            set
            {
                if (IndexDropdown != null)
                {
                    _maxIndex = value;
                    var options = Enumerable.Range(0, _maxIndex + 1).Select(i => $"Index {i}").ToList();
                    // Now populate dropdown
                    IndexDropdown.options.Clear();
                    IndexDropdown.AddOptions(options);
                }
            }
        }

        private void ProcessExecution()
        {
            int index;            
            switch (OperationType)
            {
                case ListOperations.Add:
                    index = IndexDropdown.value;
                    _operationsQueue.QueueOperation(new QueuedListOperation(ListOperations.Add, index, CodePreviewView));
                    break;
                case ListOperations.CopyOne:
                    _operationsQueue.QueueOperation(new QueuedListOperation(ListOperations.CopyOne, CodePreviewView));
                    break;
                case ListOperations.CopyDouble:
                    _operationsQueue.QueueOperation(new QueuedListOperation(ListOperations.CopyDouble, CodePreviewView));
                    break;
                case ListOperations.Delete:
                    index = IndexDropdown.value;
                    _operationsQueue.QueueOperation(new QueuedListOperation(ListOperations.Delete, index, CodePreviewView));
                    break;
                case ListOperations.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum ListOperations
    {
        Add,
        CopyOne,
        CopyDouble,
        Delete,
        Reset,
    }
}
