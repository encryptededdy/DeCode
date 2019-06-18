using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ArrayLevelManager : LevelManager
    {
        public List<IsoTransform> CarParks;
        public IsoTransform TempVarTile;

        public void WriteToArray(GameObject vehicle, int index, Action<bool> callback = null)
        {
            Vector3 position = ConvertTileToPosition(CarParks[index]);
            StartCoroutine(Destroy(position, status =>
            {
                if (status)
                {
                    Debug.Log("Successfully overwritten");
                }
                else
                {
                    Debug.Log("No need to overwrite");
                }
            }));
            StartCoroutine(MoveTo(vehicle, position, callback));
        }

        public new void Spawn(VehicleType vehicleType, Action<GameObject> callback = null)
        {
            StartCoroutine(base.Spawn(vehicleType, callback));
        }

        public void CopyFromIndexToTempVar(int index, Action<bool> callback = null)
        {
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(CarParks[index]));
            if (vehicle != null)
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(Destroy(ConvertTileToPosition(TempVarTile)));
                    StartCoroutine(MoveTo(clone, ConvertTileToPosition(TempVarTile), callback));
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void CopyFromIndexToIndex(int fromIndex, int toIndex, Action<bool> callback = null)
        {
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(CarParks[fromIndex]));
            if (vehicle != null)
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(Destroy(ConvertTileToPosition(CarParks[toIndex])));
                    StartCoroutine(MoveTo(clone, ConvertTileToPosition(CarParks[toIndex]), callback));
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void CopyFromTempVarToIndex(int index, Action<bool> callback = null)
        {
            GameObject vehicle = GetVehicleAtPosition(ConvertTileToPosition(TempVarTile));
            IsoTransform isoTransform = CarParks[index];

            if (vehicle != null)
            {
                GameObject clone = Instantiate(vehicle);
                if (AddVehicle(clone, GetVehicleType(vehicle)))
                {
                    StartCoroutine(Destroy(ConvertTileToPosition(isoTransform)));
                    StartCoroutine(MoveTo(clone, ConvertTileToPosition(isoTransform), callback));
                }
            }
            else
            {
                callback?.Invoke(false);
            }
        }

        public void Destroy(int index, Action<bool> callback = null)
        {
            IsoTransform isoTransform = CarParks[index];
            StartCoroutine(base.Destroy(ConvertTileToPosition(isoTransform), callback));
        }

        public List<VehicleType> GetArrayState()
        {
            List<VehicleType> array = new List<VehicleType>();

            foreach (IsoTransform carPark in CarParks)
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