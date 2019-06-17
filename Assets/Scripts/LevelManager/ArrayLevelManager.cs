using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public class ArrayLevelManager : LevelManager
    {
        public List<IsoTransform> CarParks;
        public IsoTransform _tempVar;

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void WriteToArray(GameObject vehicle, int index, Action<bool> callback)
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
                StartCoroutine(MoveTo(vehicle, position, callback));
            }));
        }

        public void CopyToTempVariable(GameObject vehicle, Action<bool> callback)
        {
            if (vehicle != null)
            {
                VehicleClones clones = vehicle.GetComponent<VehicleClones>();
                if (!clones.HasClone())
                {
                    GameObject clone = Instantiate(vehicle);
                    clones.AddClone(clone);
                    StartCoroutine(MoveTo(clone, ConvertTileToPosition(_tempVar), callback)); 
                }
                else
                {
                    Debug.Log("Already has references at tempVar");
                }
            }
            else
            {
                callback(false);
            }
        }

        public void Destroy(int index, Action<bool> callback)
        {
            IsoTransform isoTransform = CarParks[index];
            StartCoroutine(base.Destroy(ConvertTileToPosition(isoTransform), callback));
        }
    }
}