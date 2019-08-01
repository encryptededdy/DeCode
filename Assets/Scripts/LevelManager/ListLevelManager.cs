using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    /*
     * This class has all functionality from ArrayLevelManager (uses most of the inherited methods but not all) and
     * it is used by the list level.
     */
    public class ListLevelManager : ArrayLevelManager
    {
        private ListCarparkManager _currentListCarpark;
        private Vector3 _shiftAmount;

        /*
         * This method is called as the scene is loaded (used for setup)
         */
        protected override void OnAwake()
        {
            GridGraph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
        }

        /*
         * Creates a new list carpark of the defined size, it also moves the cars from the existing carpark to the new 
         * carpark and removes the old carpark afterwards. The callback will only return true if operations are
         * successfully performed.
         */
        public void CreateNewCarpark(int size, Action<bool> callback)
        {
            if (size > GetMaxListSize())
            {
                Debug.Log($"Maximum carpark size is {Carpark.GetComponent<ListCarparkManager>().Carparks.Count}");
                callback(false);
            }
            else
            {
                GameObject carpark = Instantiate(Carpark);
                IsoTransform isoTransform = carpark.GetComponent<IsoTransform>();
                _shiftAmount = new Vector3(isoTransform.Size.x, 0, 0);

                if (_currentListCarpark != null)
                {
                    isoTransform.Position = _currentListCarpark.GetComponent<IsoTransform>().Position + _shiftAmount;
                }

                ListCarparkManager newCarparkManager = carpark.GetComponent<ListCarparkManager>();
                if (newCarparkManager.CreateCarpark(size, out List<IsoTransform> newCarpark))
                {
                    GridGraph.UpdateGraph();
                    StartCoroutine(CopyVehiclesToNewCarpark(newCarparkManager, newCarpark, callback));
                }
                else
                {
                    DestroyImmediate(carpark);
                }
            }
        }

        /*
         * This method copies the existing car from the old to the new carpark. If number of cars is greater than
         * capacity of the new carpark, then they are destroyed. This is a coroutine which provides a callback so the
         * animation can be queued up.
         */
        private IEnumerator CopyVehiclesToNewCarpark(ListCarparkManager newCarparkManager,
            List<IsoTransform> newCarpark, Action<bool> callback)
        {
            yield return StartCoroutine(TransitionManager.SpawnCarparkEffect(newCarparkManager.GroundTiles,
                newCarparkManager.Decorations));
            if (_currentListCarpark != null)
            {
                int completed = 0;
                int expected = ActiveCarpark.Count;
                for (int i = 0; i < ActiveCarpark.Count; i++)
                {
                    if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[i]), out GameObject vehicle))
                    {
                        if (i < newCarpark.Count)
                        {
                            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(newCarparkManager.Carparks[i]),
                                status =>
                                {
                                    if (status)
                                    {
                                        completed++;
                                    }
                                    else
                                    {
                                        Debug.Log("Failed to copy cars");
                                        callback(false);
                                    }
                                }));
                        }
                        else
                        {
                            Destroy(i, status =>
                            {
                                if (status)
                                {
                                    completed++;
                                }
                                else
                                {
                                    Debug.Log("Failed to destroy car when not list is smaller");
                                    callback(false);
                                }
                            });
                        }
                    }
                    else
                    {
                        expected--;
                    }
                }

                yield return new WaitUntil(() => completed == expected);
                Debug.Log("Finished copying cars");

                yield return TransitionManager.PanCameraEffect(FindObjectOfType<Camera>(), _shiftAmount);
                yield return StartCoroutine(TransitionManager.DestroyCarparkEffect(_currentListCarpark.GroundTiles,
                    _currentListCarpark.Decorations));
                DestroyImmediate(_currentListCarpark.gameObject);
            }

            _currentListCarpark = newCarparkManager;
            ActiveCarpark = newCarpark;
            SetNewSpawnPoint(_currentListCarpark.SpawnTile);
            SetNewDestroyPoint(_currentListCarpark.DestroyTile);
            GridGraph.UpdateGraph();
            callback?.Invoke(true);
        }

        /*
         * This method returns the maxSize carpark that the list level manager can create.
         */
        public int GetMaxListSize()
        {
            return ListCarparkManager.GetMaxSize();
        }
    }
}