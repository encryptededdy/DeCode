using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEditor;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public abstract class Level : MonoBehaviour
    {
        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;
        private Vector3 _spawnPoint;
        private Vector3 _destroyPoint;

        // Only using the key as we want a thread-safe DS with ability to lookup in O(1).
        public ConcurrentDictionary<GameObject, byte> _vehicles;
        private List<string> _vehicleAssets;

        protected Level()
        {
            _vehicleAssets = AssetFinder.VehicleAssets();
            _vehicles = new ConcurrentDictionary<GameObject, byte>();
        }

        void Awake()
        {
            _spawnPoint = new Vector3(SpawnTile.Position.x, SpawnTile.Position.y + SpawnTile.Size.y,
                SpawnTile.Position.z);
            _destroyPoint = new Vector3(DestroyTile.Position.x, DestroyTile.Position.y + DestroyTile.Size.y,
                DestroyTile.Position.z);
        }

        public GameObject Spawn()
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(_spawnPoint))
                {
                    return null;
                }
            }

            int rand = RandomNumberGenerator.GetNext(0, _vehicleAssets.Count);
            GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(_vehicleAssets[rand], typeof(GameObject));
            GameObject obj = Instantiate(prefab);

            if (obj != null)
            {
                obj.GetComponent<IsoTransform>().Position.Set(_spawnPoint.x, _spawnPoint.y, _spawnPoint.z);
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                _vehicles.TryAdd(obj, 0);
            }

            return obj;
        }

        protected void Destroy(Vector3 position)
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    CustomAStarAgent customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
                    customAStarAgent.MoveTo(_destroyPoint);

                    byte b;
                    if (_vehicles.TryRemove(vehicle, out b))
                    {
                        StartCoroutine(Exit(vehicle));
                    }

                    return;
                }
            }
        }

        private IEnumerator Exit(GameObject vehicle)
        {
            while (!vehicle.GetComponent<IsoTransform>().Position.Equals(_destroyPoint))
            {
                yield return null;
            }
            DestroyImmediate(vehicle);
        }
    }
}