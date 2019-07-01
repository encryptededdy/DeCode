using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public class QueueLevelManager : LevelManager
    {
        public GameObject HeadTile;
        public GameObject Building;
        public List<IsoTransform> HideCarTiles;
        private bool _isCircularQueue;
        private int _head;
        private int _carInQueue;

        protected override void OnAwake()
        {
            _isCircularQueue = false;
            _head = 0;
            HeadTile = Instantiate(HeadTile);
            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[_head].Position;
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
            StartCoroutine(Transitions.SpawnCarparkEffect(Carpark));
        }

        void Update()
        {
            CheckIfCarIsEnteringOrExitingCarpark();
        }

        public void Spawn(Action<GameObject> callback, VehicleType vehicleType = VehicleType.random)
        {
            if (!_carInQueue.Equals(ActiveCarpark.Count))
            {
                StartCoroutine(Spawn(vehicleType, callback));
                return;
            }

            Debug.Log("Queue is full");
            callback(null);
        }

        public void Enqueue(GameObject vehicle, Action<bool> callback = null)
        {
            for (int i = 0; i < ActiveCarpark.Count; i++)
            {
                Vector3 carpark = ConvertTileToPosition(ActiveCarpark[(i + _head) % ActiveCarpark.Count]);
                if (!GetVehicleAtPosition(carpark, out GameObject _))
                {
                    StartCoroutine(WriteToIndex(vehicle, carpark, callback));
                    _carInQueue++;
                    return;
                }
            }

            callback?.Invoke(false);
        }

        public void Dequeue(Action<bool> callback)
        {
            if (_carInQueue != 0)
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
                callback(false);
            }
        }

        private IEnumerator CircularDequeue(Action<bool> callback)
        {
            StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[_head]), status =>
            {
                if (status)
                {
                    _carInQueue--;
                    callback(true);
                }
                else
                {
                    Debug.Log("Failed to dequeue car");
                    callback(false);
                }
            }));
            _head = ++_head % ActiveCarpark.Count;
            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[_head].Position;
            yield break;
        }

        private IEnumerator LinkedDequeue(Action<bool> callback)
        {
            StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[_head]), status =>
            {
                if (status)
                {
                    _carInQueue--;
                }
                else
                {
                    Debug.Log("Failed to dequeue car");
                    callback(false);
                }
            }));
            for (int i = 1; i <= _carInQueue; i++)
            {
                if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[i]), out GameObject vehicle))
                {
                    yield return StartCoroutine(MoveTo(vehicle, ConvertTileToPosition(ActiveCarpark[i - 1]),
                        status =>
                        {
                            if (!status)
                            {
                                Debug.Log("Failed to copy cars");
                                callback(false);
                            }
                        }));
                }
            }

            Debug.Log("Successfully copied cars");
            callback(true);
        }

        private void CheckIfCarIsEnteringOrExitingCarpark()
        {
            if (Building.activeSelf)
            {
                foreach (CustomAStarAgent vehicle in FindObjectsOfType<CustomAStarAgent>())
                {
                    IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                    foreach (IsoTransform tile in HideCarTiles)
                    {
                        if (Vector3Collider.Intersect(isoTransform.Position, ConvertTileToPosition(tile), tile.Size))
                        {
                            vehicle.GetComponent<Renderer>().enabled = false;
                            return;
                        }
                    }

                    vehicle.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        public void SetType(bool circular)
        {
            _isCircularQueue = circular;
        }

        public void SetHiddenImplementation(bool hidden)
        {
            Building.SetActive(!hidden);
        }

        public int getMaxQueueSize()
        {
            return ActiveCarpark.Count;
        }

        public int getNumElements()
        {
            return _carInQueue;
        }
    }
}