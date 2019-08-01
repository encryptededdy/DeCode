using System;
using System.Collections;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    /*
     * This class is used by the stack level.
     */
    public class QueueLevelManager : ADTLevelManager
    {
        private bool _isCircularQueue;
        private int _head;

        protected override void OnAwake()
        {
            _isCircularQueue = false;
            _head = 0;
            base.OnAwake();
        }

        /*
         * This method adds a new car into the carpark, mimic operation of a queue.
         */
        public void Enqueue(GameObject vehicle, Action<bool> callback)
        {
            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[_head].Position;
            HeadTile.SetActive(true);
            StartCoroutine(WriteToIndex(vehicle,
                ConvertTileToPosition(ActiveCarpark[(NumElements + _head) % ActiveCarpark.Count]), callback));
            NumElements++;
        }

        /**
         * Dequeue a car from the carpark (see CircularDequeue or LinkedDequeue)
         */
        public void Dequeue(Action<bool> callback)
        {
            if (NumElements != 0)
            {
                if (_isCircularQueue)
                {
                    StartCoroutine(CircularDequeue(callback));
                }
                else
                {
                    StartCoroutine(LinkedDequeue(callback));
                }
            }
            else
            {
                Debug.Log("Queue is empty");
                callback(true);
            }
        }
        
        /*
         * Used by the circular implementation to dequeue a car (where the head tile moves and the cars does not need
         * to be copied)
         */
        private IEnumerator CircularDequeue(Action<bool> callback)
        {
            NumElements--;
            yield return StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[_head]), status =>
            {
                if (status)
                {
                    if (NumElements == 0)
                    {
                        _head = 0;
                        HeadTile.SetActive(false);
                    }
                    else
                    {
                        _head = ++_head % ActiveCarpark.Count;
                        HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[_head].Position;
                    }

                    callback(true);
                }
                else
                {
                    Debug.Log("Failed to dequeue vehicle");
                    callback(false);
                }
            }));
        }

        /*
         * Used by the default implementation to dequeue a car (where the head tile does not move) and all cars
         * shifts across by 1.
         */
        private IEnumerator LinkedDequeue(Action<bool> callback)
        {
            NumElements--;
            StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[_head]), status =>
            {
                if (status)
                {
                    if (NumElements == 0)
                    {
                        HeadTile.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Failed to dequeue vehicle");
                    callback(false);
                }
            }));
            for (int i = 1; i <= NumElements; i++)
            {
                if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[i]), out GameObject vehicle))
                {
                    yield return MoveTo(vehicle, ConvertTileToPosition(ActiveCarpark[i - 1]),
                        status =>
                        {
                            if (!status)
                            {
                                Debug.Log("Failed to copy vehicles");
                                callback(false);
                            }
                        });
                }
            }

            Debug.Log("Successfully copied vehicles");
            callback(true);
        }

        /*
         * Change the queue from the default implementation to a circular implementation
         */
        public void SetCircularQueue(bool circular)
        {
            _isCircularQueue = circular;
        }
    }
}