using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEditor;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public abstract class LevelManager : MonoBehaviour
    {
        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;
        private Vector3 _spawnPoint;
        private Vector3 _destroyPoint;

        // Only using the key as we want a thread-safe DS with ability to lookup in O(1).
        public ConcurrentDictionary<GameObject, byte> _vehicles;
        private readonly List<string> _vehicleAssets;

        protected LevelManager()
        {
            _vehicleAssets = AssetFinder.VehicleAssets();
            _vehicles = new ConcurrentDictionary<GameObject, byte>();
        }

        void Awake()
        {
            _spawnPoint = ConvertTileToPosition(SpawnTile);
            _destroyPoint = ConvertTileToPosition(DestroyTile);
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
                obj.GetComponent<IsoTransform>().Position = _spawnPoint;
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                if (!_vehicles.TryAdd(obj, 0))
                {
                    return null;
                }
            }

            return obj;
        }

        protected void MoveTo(GameObject vehicle, Vector3 position)
        {
            CustomAStarAgent customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
            customAStarAgent.MoveTo(position);
        }

        protected bool Destroy(Vector3 position)
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    MoveTo(vehicle, _destroyPoint);
                    byte b;
                    if (_vehicles.TryRemove(vehicle, out b))
                    {
                        StartCoroutine(Exit(vehicle));
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator Exit(GameObject vehicle)
        {
            yield return new WaitUntil(() => vehicle.GetComponent<IsoTransform>().Position.Equals(_destroyPoint));
            DestroyImmediate(vehicle);
        }

        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }
    }
}