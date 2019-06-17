using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public class ArrayLevel : Level
    {
        public List<IsoTransform> CarParks;
        private ConcurrentDictionary<String, IsoTransform> _tempVar;

        public ArrayLevel()
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

        public void Destroy(int index)
        {
            IsoTransform isoTransform = CarParks[index];
            Destroy(new Vector3(isoTransform.Position.x, isoTransform.Size.y + isoTransform.Position.y, isoTransform.Position.z));
            Debug.Log(_vehicles.Count);
        }
        
        public void Destroy(string var)
        {
            
        }
    }
}