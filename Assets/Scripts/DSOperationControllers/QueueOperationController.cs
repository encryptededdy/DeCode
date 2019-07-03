using System;
using UnityEngine;
using UnityEngine.UI;

namespace DSOperationControllers
{
    public class QueueOperationController: MonoBehaviour
    {
        public Button ExecuteButton;
        public QueueOperations QueueOperation;
        public Text Code;
        public QueueOperationQueue Queue;

        private void Start()
        {
            ExecuteButton.onClick.AddListener(() =>
            {
                switch (QueueOperation)
                {
                    case QueueOperations.Enqueue:
                        Queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Enqueue, Code));
                        break;
                    case QueueOperations.EnqueueSpecific:
                        break;
                    case QueueOperations.Dequeue:
                        Queue.QueueOperation(new QueuedQueueOperation(QueueOperations.Dequeue, Code));
                        break;
                    case QueueOperations.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}