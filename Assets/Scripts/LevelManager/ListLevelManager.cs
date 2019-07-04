using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        private ListCarparkManager _currentListCarpark;
        private Vector3 _shiftAmount;

        protected override void OnAwake()
        {
            GridGraph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
        }

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
                            Destroy(i,
                                status =>
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


        public int GetMaxListSize()
        {
            return ListCarparkManager.GetMaxSize();
        }
    }
}