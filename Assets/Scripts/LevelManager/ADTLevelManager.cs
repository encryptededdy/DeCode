using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    /*
     * This class contains common methods which are used for all ADT levels (Queue and Stack)
     */
    public class ADTLevelManager : LevelManager
    {
        public GameObject HeadTile;
        public GameObject Building;
        public List<IsoTransform> HideCarTiles;
        protected int NumElements;

        /*
         * This is method called as the scene is loaded (used for setup)
         */
        protected override void OnAwake()
        {
            // Need to disable the red flags used as pointer at the start of the level
            NumElements = 0;
            HeadTile.SetActive(false);
            
            // Set the spawn/destroy point to the box above the spawn/destroy tile
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
        }
        
        /*
         * This method contains custom spawn animation for starting the level. Once completed, the callback will return
         * true else if there are errors, it returns false.
         */
        public new void StartLevel(Action<bool> callback)
        {
            // Set the building active for the spawn animations
            Building.SetActive(true);
            base.StartLevel(callback);
        }

        /*
         * Update gets called every frame
         */
        private void Update()
        {
            CheckIfCarIsEnteringOrExitingCarpark();
        }

        /*
         * Difference between this method and the default Spawn is that it needs to check if the carpark is full.
         */
        public void Spawn(Action<Tuple<VehicleType, GameObject>> callback, VehicleType vehicleType = VehicleType.random)
        {
            if (!NumElements.Equals(GetMaxSize()))
            {
                StartCoroutine(Spawn(vehicleType, callback));
                return;
            }

            Debug.Log("ADT is full");
            callback(null);
        }
        
        /*
         * This method disable or enable the building so the underlying implementation can be exposed.
         */
        public void SetHiddenImplementation(bool hidden)
        {
            Building.SetActive(!hidden);
        }

        /*
         * Difference between this method and the default Spawn is that it needs to reset the red flags used for pointer
         */
        public new void ResetLevel(Action<bool> callback, bool fast = false)
        {
            NumElements = 0;
            HeadTile.SetActive(false);
            base.ResetLevel(callback, fast);
        }

        /*
         * Gets the maximum size of the carpark
         */
        public int GetMaxSize()
        {
            return ActiveCarpark.Count;
        }

        /*
         * Gets the current number of cars in the carpark
         */
        public int GetNumElements()
        {
            return NumElements;
        }
        
        /*
         * Due to complexity of the sorting logic, we need to hide the car entering the carpark if the building is
         * enabled so that it appears as if the car is entering the carpark rather than going over or under the
         * entrance/exit tiles
         *
         * HideCarTiles is a collection of gameObjects defined in the manager of the respective level.
         */
        private void CheckIfCarIsEnteringOrExitingCarpark()
        {
            if (Building.activeSelf)
            {
                foreach (CustomAStarAgent vehicle in FindObjectsOfType<CustomAStarAgent>())
                {
                    IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                    foreach (IsoTransform tile in HideCarTiles)
                    {
                        // Check if is intersecting the entrance/exit tiles (or any tiles added to HideCarTiles
                        if (Vector3Collider.Intersect(isoTransform.Position, ConvertTileToPosition(tile), tile.Size))
                        {
                            vehicle.GetComponent<Renderer>().enabled = false;
                            vehicle.GetComponent<CustomVehicleAnimator>().PointOfInterestIndicator.GetComponent<Renderer>().enabled = false;
                            return;
                        }
                    }

                    vehicle.GetComponent<Renderer>().enabled = true;
                    vehicle.GetComponent<CustomVehicleAnimator>().PointOfInterestIndicator.GetComponent<Renderer>().enabled = true;
                }
            }
        }
    }
}