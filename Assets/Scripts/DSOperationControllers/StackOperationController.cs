using System;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class StackOperationController: MonoBehaviour
    {
        public Button ExecuteButton;
        public StackOperations StackOperation;
        public Text Code;
        public StackOperationQueue Queue;

        private void Start()
        {
            ExecuteButton.onClick.AddListener(() =>
            {
                switch (StackOperation)
                {
                    case StackOperations.Push:
                        Queue.QueueOperation(new QueuedStackOperation(StackOperations.Push, Code));
                        break;
                    case StackOperations.Pop:
                        Queue.QueueOperation(new QueuedStackOperation(StackOperations.Pop, Code));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}