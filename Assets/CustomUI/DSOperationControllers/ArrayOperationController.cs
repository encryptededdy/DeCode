using System;
using LevelManager;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.DSOperationControllers
{
    public class ArrayOperationController : MonoBehaviour
    {
        public Button ExecuteOperationButton;
        public InputField IndexInputField;
        public InputField IndexInputField2 = null;
        public Text CodePreviewView;
        public ArrayOperations OperationType;
        public ArrayLevelManager LevelManager;
        public int NumberOfParks;
        
        // Start is called before the first frame update
        void Start()
        {
            ExecuteOperationButton.onClick.AddListener(ProcessExecution);   
        }

        private void ProcessExecution()
        {
            if (IndexInputField.text.Length == 0)
            {
                CodePreviewView.text = "Invalid Index";
                return;
            }

            var index = int.Parse(IndexInputField.text);

            if (index >= NumberOfParks)
            {
                CodePreviewView.text = "Index out of range";
                return;
            }
            
            switch (OperationType)
            {
                case ArrayOperations.Add:
                    CodePreviewView.text = "car = new Car();\n" +
                                           $"array[{index}] = car;";
                    LevelManager.Spawn(obj => { LevelManager.WriteToArray(obj, index); });
                    break;
                case ArrayOperations.ToTemp:
                    CodePreviewView.text = $"temp = array[{index}];";
                    LevelManager.CopyFromIndexToTempVar(index);
                    break;
                case ArrayOperations.FromTemp:
                    CodePreviewView.text = $"array[{index}] = temp;";
                    LevelManager.CopyFromTempVarToIndex(index);
                    break;
                case ArrayOperations.Delete:
                    CodePreviewView.text = $"array[{index}] = null;";
                    LevelManager.Destroy(index);
                    break;
                case ArrayOperations.CopyTo:
                    // Parse the second field
                    if (IndexInputField2.text.Length == 0)
                    {
                        CodePreviewView.text = "Invalid Index";
                        return;
                    }

                    var index2 = int.Parse(IndexInputField2.text);

                    if (index2 >= NumberOfParks)
                    {
                        CodePreviewView.text = "Index out of range";
                        return;
                    }
                    
                    // Perform the copy
                    CodePreviewView.text = $"array[{index2}] = array[{index}];";
                    LevelManager.CopyFromIndexToIndex(index, index2);
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
