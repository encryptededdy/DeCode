using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    /*
     * This class contains common methods which are used in Array and List (as List is just an extension to Array)
     */
    public class ArrayLevelManager : LevelManager
    {
        public IsoTransform TempVarTile;

        /*
         * This method is called as the scene is loaded (used for setup)
         */
        protected override void OnAwake()
        {
            // Set the spawn/destroy point to the box above the spawn/destroy tile
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
        }
        
        /*
         * Spawns a new car at the spawn tile (defined in the respective level manager)
         * The callback returns a tuple of the VehicleType of the spawned car as well as the reference to the spawned
         * instance.
         */
        public void Spawn(Action<Tuple<VehicleType, GameObject>> callback, VehicleType vehicleType = VehicleType.random)
        {
            StartCoroutine(Spawn(vehicleType, callback));
        }

        /*
         * Moves a vehicle to the specified index of the array (usually used in combination with the Spawn method). The
         * car originally occupying the index will be destroyed if operation is successful.
         */
        public void WriteToArray(GameObject vehicle, int index, Action<bool> callback, bool fast = false)
        {
            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(ActiveCarpark[index]), callback, fast));
        }

        /*
         * Copy the car at the specified index to the temp carpark. The car originally occupying the temp carpark
         * will be destroyed if operation is successful.
         *
         * If the car is unable to move to destination or there are no car occupying the specified index, the callback
         * with return false.
         */
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

        /*
         * Copy the car at the fromIndex to the index position of the toIndex. The car originally occupying the toIndex
         * will be destroyed if operation is successful.
         *
         * If the car is unable to move to destination or there are no car occupying the fromIndex, the callback with
         * return false.
         */
        public void CopyFromIndexToIndex(int fromIndex, int toIndex, Action<bool> callback)
        {
            if (fromIndex == toIndex)
            {
                callback?.Invoke(true);
                return;
            }
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
        
        
        /*
         * Copy the car at the temp carpark to the specified index. The car originally occupying the specified index
         * will be destroyed if operation is successful.
         *
         * If the car is unable to move to destination or there are no car occupying the specified index, the
         * callback with return false.
         */
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
        
        /*
         * Destroy is used to remove the car at the specified index from the carpark (clearing reference)
         */

        public void Destroy(int index, Action<bool> callback, bool fast = false)
        {
            StartCoroutine(base.Destroy(ConvertTileToPosition(ActiveCarpark[index]), callback, fast));
        }

        /*
         * Returns the size of the array
         */
        public int GetArraySize()
        {
            return ActiveCarpark.Count;
        }

        /*
         * Returns the state of the array (in VehicleTypes), it will return VehicleType.empty if the the particular
         * index is not occupied by a car
         */
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