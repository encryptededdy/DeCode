using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public class ADTLevelManager : LevelManager
    {
        public GameObject HeadTile;
        public GameObject Building;
        public List<IsoTransform> HideCarTiles;
        protected int NumElements;

        protected override void OnAwake()
        {
            NumElements = 0;
            HeadTile.SetActive(false);
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
            StartCoroutine(Transitions.SpawnCarparkEffect(Carpark));
        }

        void Update()
        {
            CheckIfCarIsEnteringOrExitingCarpark();
        }

        public void Spawn(Action<Tuple<VehicleType, GameObject>> callback, VehicleType vehicleType = VehicleType.random)
        {
            if (!NumElements.Equals(ActiveCarpark.Count))
            {
                StartCoroutine(Spawn(vehicleType, callback));
                return;
            }

            Debug.Log("ADT is full");
            callback(null);
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

        public void SetHiddenImplementation(bool hidden)
        {
            Building.SetActive(!hidden);
        }

        public int GetMaxSize()
        {
            return ActiveCarpark.Count;
        }

        public int GetNumElements()
        {
            return NumElements;
        }
    }
}