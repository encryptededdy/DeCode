using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class ArrayLevelManager : LevelManager
    {
        public List<IsoTransform> CarParks;
        private ConcurrentDictionary<String, IsoTransform> _tempVar;

        public ArrayLevelManager()
        {
            _tempVar = new ConcurrentDictionary<string, IsoTransform>();
        }

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
            }));
            StartCoroutine(MoveTo(vehicle, position, callback));
        }

        public void DestroyElement(int index, Action<bool> callback)
        {
            IsoTransform isoTransform = CarParks[index];
            StartCoroutine(Destroy(ConvertTileToPosition(isoTransform), callback));
        }

        public bool DestroyVariable(string variable)
        {
            //TODO
            return false;
        }
    }
}