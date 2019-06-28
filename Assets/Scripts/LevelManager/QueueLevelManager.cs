using System;
using UnityEngine;

namespace LevelManager
{
    public class QueueLevelManager : LevelManager
    {
        public Sprite HeadTile;
        public Sprite CarparkTile;
        public GameObject Building;
        private bool _circularQueue;
        private int head;

        protected override void OnAwake()
        {
            _circularQueue = false;
            head = 0;
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
        }

        public void Spawn(Action<GameObject> callback, VehicleType vehicleType = VehicleType.random)
        {
            StartCoroutine(Spawn(vehicleType, callback));
        }

        public void Enqueue(GameObject vehicle, Action<bool> callback = null)
        {
            for (int i = 0; i < ActiveCarpark.Count; i++)
            {
                Vector3 carpark = ConvertTileToPosition(ActiveCarpark[(i + head) % ActiveCarpark.Count]);
                Debug.Log(ActiveCarpark[(i + head) % ActiveCarpark.Count]);
                if (!GetVehicleAtPosition(carpark, out GameObject _))
                {
                    StartCoroutine(WriteToIndex(vehicle, carpark, callback));
                    return;
                }
            }

            callback?.Invoke(false);
        }

        public void Dequeue(Action<bool> callback)
        {
        }
    }
}