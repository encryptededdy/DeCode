using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ArrayLevelManager : LevelManager
    {
        public IsoTransform TempVarTile;

        protected override void OnAwake()
        {
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
            StartCoroutine(TransitionManager.SpawnCarparkEffect(Carpark, Decorations));
        }

        public void Spawn(Action<Tuple<VehicleType, GameObject>> callback, VehicleType vehicleType = VehicleType.random)
        {
            StartCoroutine(Spawn(vehicleType, callback));
        }

        public void WriteToArray(GameObject vehicle, int index, Action<bool> callback, bool fast = false)
        {
            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(ActiveCarpark[index]), callback, fast));
        }

        public void CopyFromIndexToTempVar(int index, Action<bool> callback)
        {
            if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[index]), out GameObject vehicle))
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(WriteToIndex(clone, ConvertTileToPosition(TempVarTile), callback));
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void CopyFromIndexToIndex(int fromIndex, int toIndex, Action<bool> callback)
        {
            if (GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[fromIndex]), out GameObject vehicle))
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(WriteToIndex(clone, ConvertTileToPosition(ActiveCarpark[toIndex]), callback));
                }

                else
                {
                    callback?.Invoke(true);
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void CopyFromTempVarToIndex(int index, Action<bool> callback)
        {
            IsoTransform isoTransform = ActiveCarpark[index];

            if (GetVehicleAtPosition(ConvertTileToPosition(TempVarTile), out GameObject vehicle))
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(WriteToIndex(clone, ConvertTileToPosition(isoTransform), callback));
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void Destroy(int index, Action<bool> callback, bool fast = false)
        {
            StartCoroutine(base.Destroy(ConvertTileToPosition(ActiveCarpark[index]), callback, fast));
        }

        public int GetArraySize()
        {
            return ActiveCarpark.Count;
        }

        public List<VehicleType> GetArrayState()
        {
            List<VehicleType> array = new List<VehicleType>();

            foreach (var carPark in ActiveCarpark)
            {
                if (!GetVehicleAtPosition(ConvertTileToPosition(carPark), out GameObject vehicle))
                {
                    array.Add(VehicleType.empty);
                }
                else
                {
                    array.Add(GetVehicleType(vehicle));
                }
            }

            return array;
        }
    }
}