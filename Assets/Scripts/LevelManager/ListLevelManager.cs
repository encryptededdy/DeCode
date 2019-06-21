using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        public List<IsoTransform> CarParkMedium;
        public IsoTransform MediumSpawnTile;
        public IsoTransform MediumDestroyTile;

        public List<IsoTransform> CarParkLarge;
        public IsoTransform LargeSpawnTile;
        public IsoTransform LargeDestroyTile;

        public List<IsoTransform> CustomMapLayout;

        private Stage stage;

        protected void Awake()
        {
            stage = Stage.One;
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

        public void NextStage(Action<bool> callback)
        {
            if (GetNextOpenIndex() != -1)
            {
                Debug.Log("Carpark has not been filled");
                callback(false);
                return;
            }

            switch (stage)
            {
                case Stage.One:
                    SetNewSpawnPoint(MediumSpawnTile);
                    SetNewDestroyPoint(MediumDestroyTile);
                    SetNewActiveCarpark(CarParkMedium);
                    stage = Stage.Two;
                    break;
                case Stage.Two:
                    SetNewSpawnPoint(LargeSpawnTile);
                    SetNewDestroyPoint(LargeDestroyTile);
                    SetNewActiveCarpark(CarParkLarge);
                    stage = Stage.Three;
                    break;
                case Stage.Three:
                    Debug.Log("Level Complete");
                    break;
            }

            callback(true);
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