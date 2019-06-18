using System;
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
    public abstract class LevelManager : MonoBehaviour
    {
        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;
        private Vector3 _spawnPoint;
        private Vector3 _destroyPoint;

        // Only using the key as we want a thread-safe DS with ability to lookup in O(1).
        private ConcurrentDictionary<GameObject, byte> _vehicles;
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

        protected IEnumerator Spawn(Action<GameObject> callback = null)
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(_spawnPoint))
                {
                    callback?.Invoke(null);
                    yield break;
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
                    callback?.Invoke(null);
                    yield break;
                }
            }
            Debug.Log("There are " + _vehicles.Count + " active vehicles");
            callback?.Invoke(obj);
        }

        protected IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback = null)
        {
            CustomAStarAgent customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
            yield return StartCoroutine(customAStarAgent.MoveTo(position, callback));
        }

        protected IEnumerator Destroy(Vector3 position, Action<bool> callback = null)
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    byte b;
                    if (_vehicles.TryRemove(vehicle, out b))
                    {
                        yield return MoveTo(vehicle, _destroyPoint, callback);
                        DestroyImmediate(vehicle);
                        Debug.Log("There are " + _vehicles.Count + " active vehicles");
                        yield break;
                    }
                }
            }
            callback?.Invoke(false);
        }
        
        protected GameObject GetVehicleAtPosition(Vector3 position)
        {
            foreach (GameObject vehicle in _vehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    return vehicle;
                }
            }

            return null;
        }
        
        protected bool AddVehicle(GameObject vehicle)
        {
            bool added = _vehicles.TryAdd(vehicle, 0);
            Debug.Log("There are " + _vehicles.Count + " active vehicles");
            return added;
        }

        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }
    }
}