using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        public ListCarparkManager CurrentListCarpark;
        public ListCarparkManager NewListCarpark;
        private Vector3 _shiftAmount;

        protected override void OnAwake()
        {
        }

        public new void Spawn(Action<GameObject> callback, VehicleType vehicleType = VehicleType.random)
        {
            if (CurrentListCarpark == null)
            {
                Debug.Log("There are no active carparks");
                callback?.Invoke(null);
            }
            else
            {
                base.Spawn(callback, vehicleType);
            }
        }

        public void CreateNewCarpark(int size, Action<bool> callback)
        {
            if (size > Carpark.GetComponent<ListCarparkManager>().Carparks.Count)
            {
                Debug.Log($"Maximum carpark size is {Carpark.GetComponent<ListCarparkManager>().Carparks.Count}");
                callback(false);
            }
            else
            {
                GameObject carpark = Instantiate(Carpark);
                IsoTransform isoTransform = carpark.GetComponent<IsoTransform>();
                _shiftAmount = new Vector3(isoTransform.Size.x, 0, 0);

                if (CurrentListCarpark != null)
                {
                    isoTransform.Position = CurrentListCarpark.GetComponent<IsoTransform>().Position + _shiftAmount;
                }

                NewListCarpark = carpark.GetComponent<ListCarparkManager>();
                for (int i = 0; i < size; i++)
                {
                    NewListCarpark.AddCarpark();
                }

                GridGraph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                GridGraph.UpdateGraph();

                StartCoroutine(CopyVehiclesToNewCarpark(callback));
            }
        }

        private IEnumerator CopyVehiclesToNewCarpark(Action<bool> callback)
        {
            yield return StartCoroutine(SpawnCarparkEffect());
            if (CurrentListCarpark != null)
            {
                int completed = 0;
                int expected = ActiveCarpark.Count;
                for (int i = 0; i < ActiveCarpark.Count; i++)
                {
                    if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[i]), out GameObject vehicle))
                    {
                        if (i < NewListCarpark.GetSize())
                        {
                            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(NewListCarpark.Carparks[i]),
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

                yield return Transitions.PanCamera(FindObjectOfType<Camera>(), _shiftAmount);
                yield return StartCoroutine(DestroyCarparkEffect());
            }

            CurrentListCarpark = NewListCarpark;
            ActiveCarpark = NewListCarpark.Carparks;
            SetNewSpawnPoint(NewListCarpark.SpawnTile);
            SetNewDestroyPoint(NewListCarpark.DestroyTile);
            GridGraph.UpdateGraph();
            callback?.Invoke(true);
        }

        private IEnumerator SpawnCarparkEffect()
        {
            List<GameObject> tiles = new List<GameObject>();
            foreach (Transform child in NewListCarpark.transform)
            {
                tiles.Add(child.gameObject);
                Material material = child.GetComponent<SpriteRenderer>().material;
                var color = material.color;
                color.a = 0f;
                material.color = color;
            }

            List<GameObject> shuffledList = Randomiser.ShuffleList(tiles);
            foreach (GameObject child in shuffledList)
            {
                yield return StartCoroutine(Transitions.FadeAnimation(child, Transitions.FadeDirection.In));
            }
        }

        private IEnumerator DestroyCarparkEffect()
        {
            List<GameObject> tiles = new List<GameObject>();
            foreach (Transform child in CurrentListCarpark.transform)
            {
                tiles.Add(child.gameObject);
            }

            List<GameObject> shuffledList = Randomiser.ShuffleList(tiles);
            foreach (GameObject child in shuffledList)
            {
                yield return StartCoroutine(Transitions.FadeAnimation(child, Transitions.FadeDirection.Out));
            }

            DestroyImmediate(CurrentListCarpark.gameObject);
            Debug.Log("Destroyed old carpark");
        }

        public new int GetArraySize()
        {
            return CurrentListCarpark == null ? 0 : CurrentListCarpark.GetSize();
        }

        public int GetMaxListSize()
        {
            return ListCarparkManager.GetMaxSize();
        }
    }
}