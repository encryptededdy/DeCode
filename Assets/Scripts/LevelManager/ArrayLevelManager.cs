using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ArrayLevelManager : LevelManager
    {
        public IsoTransform TempVarTile;

        protected void Awake()
        {
            base.Awake();
        }

        public void WriteToArray(GameObject vehicle, int index, Action<bool> callback, bool fast = false)
        {
            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(ActiveCarpark[index]), callback, fast));
        }

        public void Spawn(Action<GameObject> callback, VehicleType vehicleType = VehicleType.random)
        {
            StartCoroutine(base.Spawn(vehicleType, callback));
        }

        public void CopyFromIndexToTempVar(int index, Action<bool> callback)
        {
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[index]));
            if (vehicle != null)
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
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(ActiveCarpark[fromIndex]));
            if (vehicle != null)
            {
                if (fromIndex != toIndex)
                {
                    GameObject clone = Instantiate(vehicle);
                    if (AddVehicle(clone, GetVehicleType(vehicle)))
                    {
                        StartCoroutine(WriteToIndex(clone, ConvertTileToPosition(ActiveCarpark[toIndex]), callback));
                    }
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
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(TempVarTile));
            IsoTransform isoTransform = ActiveCarpark[index];

            if (vehicle != null)
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

        public new void ResetLevel(Action<bool> callback = null, bool fast = false)
        {
            StartCoroutine(base.ResetLevel(callback, fast));
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
                GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(carPark));
                if (vehicle == null)
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