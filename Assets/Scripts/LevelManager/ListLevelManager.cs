using System;
using System.Collections;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        public CarparkManager CurrentCarpark;
        public CarparkManager NewCarpark;

        public GameObject Carpark;
        private CustomGridGraph.CustomGridGraph _gridGraph;
        private Vector3 _shiftAmount;

        protected override void OnAwake()
        {
        }

        public new void Spawn(Action<GameObject> callback, VehicleType vehicleType = VehicleType.random)
        {
            if (ActiveCarpark == null)
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
            if (size > Carpark.GetComponent<CarparkManager>().Carparks.Count)
            {
                Debug.Log($"Maximum carpark size is {Carpark.GetComponent<CarparkManager>().Carparks.Count}");
                callback(false);
            }
            else
            {
                GameObject carpark = Instantiate(Carpark);
                IsoTransform isoTransform = carpark.GetComponent<IsoTransform>();
                _shiftAmount = new Vector3(isoTransform.Size.x, 0, 0);

                if (CurrentCarpark != null)
                {
                    isoTransform.Position = CurrentCarpark.GetComponent<IsoTransform>().Position + _shiftAmount;
                }

                NewCarpark = carpark.GetComponent<CarparkManager>();
                for (int i = 0; i < size; i++)
                {
                    NewCarpark.AddCarpark();
                }

                _gridGraph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                _gridGraph.UpdateGraph();

                StartCoroutine(CopyVehiclesToNewCarpark(callback));
            }
        }

        private IEnumerator CopyVehiclesToNewCarpark(Action<bool> callback)
        {
            if (ActiveCarpark != null)
            {
                int completed = 0;
                int expected = ActiveCarpark.Count;
                for (int i = 0; i < ActiveCarpark.Count; i++)
                {
                    if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[i]), out GameObject vehicle))
                    {
                        StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(NewCarpark.Carparks[i]),
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
                        expected--;
                    }
                }

                yield return new WaitUntil(() => completed == expected);
                Debug.Log("Finished copying cars");

                yield return Transitions.PanCamera(FindObjectOfType<Camera>(), _shiftAmount);
                DestroyImmediate(CurrentCarpark.gameObject);
                Debug.Log("Destroyed old carpark");
            }

            CurrentCarpark = NewCarpark;
            ActiveCarpark = NewCarpark.Carparks;
            SetNewSpawnPoint(NewCarpark.SpawnTile);
            SetNewDestroyPoint(NewCarpark.DestroyTile);
            _gridGraph.UpdateGraph();
            callback?.Invoke(true);
        }
    }
}