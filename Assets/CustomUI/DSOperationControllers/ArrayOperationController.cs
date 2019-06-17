using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.DSOperationControllers
{
    public class ArrayOperationController : MonoBehaviour
    {
        public Button ExecuteOperationButton;
        public InputField IndexInputField;
        public Text CodePreviewView;
        public ArrayOperations OperationType;
        
        // Start is called before the first frame update
        void Start()
        {
            ExecuteOperationButton.onClick.AddListener(ProcessExecution);   
        }

        private void ProcessExecution()
        {
            if (IndexInputField.text.Length == 0)
                CodePreviewView.text = "Invalid Index";
            
            var index = int.Parse(IndexInputField.text);
            switch (OperationType)
            {
                case ArrayOperations.Add:
                    CodePreviewView.text = "car = new Car();\n" +
                                           $"array[{index}] = car;";
                    // TODO: Trigger Array Operation Code
                    break;
                case ArrayOperations.ToTemp:
                    CodePreviewView.text = $"temp = array[{index}];";
                    break;
                case ArrayOperations.FromTemp:
                    CodePreviewView.text = $"array[{index}] = temp;";
                    break;
                case ArrayOperations.Delete:
                    CodePreviewView.text = $"array[{index}] = null;";
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
        Delete
    }
}
