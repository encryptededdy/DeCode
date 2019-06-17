using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;
using Vehicle;

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

        public void WriteToArray(GameObject vehicle, int index)
        {
            Vector3 position = ConvertTileToPosition(CarParks[index]);
            base.Destroy(position);
            MoveTo(vehicle, position);
        }

        public bool Destroy(int index)
        {
            IsoTransform isoTransform = CarParks[index];
            return base.Destroy(ConvertTileToPosition(isoTransform));
        }
        
        public bool Destroy(string variable)
        {
            //TODO
            return false;
        }
    }
}