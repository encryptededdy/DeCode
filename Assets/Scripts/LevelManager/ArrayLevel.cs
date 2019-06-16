using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEditor;
using UnityEngine;
using Vehicle;
using Object = UnityEngine.Object;

namespace LevelManager
{
    public class ArrayLevel : MonoBehaviour
    {
        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;
        public List<IsoTransform> CarParks;

        private Vector3 SpawnPoint;
        private Vector3 DestroyPoint;
        private ConcurrentDictionary<String, IsoTransform> _tempVar;
        private readonly ConcurrentBag<GameObject> _vehicles;
        private List<string> _vehicleAssets;

        public ArrayLevel()
        {
            _vehicles = new ConcurrentBag<GameObject>();
            _tempVar = new ConcurrentDictionary<string, IsoTransform>();
        }

        // Start is called before the first frame update
        void Awake()
        {
            _vehicleAssets = AssetFinder.VehicleAssets();
            SpawnPoint = new Vector3(SpawnTile.Position.x, SpawnTile.Position.y + SpawnTile.Size.y,
                SpawnTile.Position.z);
            DestroyPoint = new Vector3(DestroyTile.Position.x, DestroyTile.Position.y + DestroyTile.Size.y,
                DestroyTile.Position.z);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Spawn()
        {
            Debug.Log(_vehicles.Count);
            foreach (GameObject vehicle in _vehicles)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(SpawnPoint))
                {
                    return;
                }
            }
            
            int rand = RandomNumberGenerator.GetNext(0, _vehicleAssets.Count);
            GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(_vehicleAssets[rand], typeof(GameObject));
            GameObject obj = Instantiate(prefab);
            
            if (obj != null)
            {
                obj.GetComponent<IsoTransform>().Position.Set(SpawnTile.Position.x, SpawnTile.Position.y + SpawnTile.Size.y, SpawnTile.Position.z);
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                _vehicles.Add(obj);
            }
        }
    }
}