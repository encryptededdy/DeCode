using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        public IsoTransform CurrentCarpark;
        public IsoTransform NewCarpark;

        protected void Awake()
        {
            base.Awake();
        }

        public void AddElement(GameObject vehicle, Action<bool> callback, bool fast = false)
        {
            int nextOpenIndex = GetNextOpenIndex();

            if (nextOpenIndex == -1)
            {
                Vector3 position = vehicle.GetComponent<IsoTransform>().Position;
                StartCoroutine(Destroy(position, status =>
                {
                    Debug.Log("Lane is full");
                    callback(false);
                }, fast));
            }
            else
            {
                WriteToArray(vehicle, nextOpenIndex, callback);
            }
        }

        public void Expand(Action<bool> callback)
        {
        }

        private int GetNextOpenIndex()
        {
            List<VehicleType> vehicleTypes = GetArrayState();

            for (int i = 0; i < vehicleTypes.Count; i++)
            {
                if (vehicleTypes[i].Equals(VehicleType.empty))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}