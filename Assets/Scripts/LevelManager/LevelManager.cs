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
        private ConcurrentDictionary<GameObject, VehicleType> _activeVehicles;
        private readonly ConcurrentDictionary<VehicleType, GameObject> _spawnableVehicles;

        protected LevelManager()
        {
            _spawnableVehicles = new ConcurrentDictionary<VehicleType, GameObject>();
            _activeVehicles = new ConcurrentDictionary<GameObject, VehicleType>();
        }

        void Awake()
        {
            _spawnPoint = ConvertTileToPosition(SpawnTile);
            _destroyPoint = ConvertTileToPosition(DestroyTile);

            // Pre-loading the assets
            Dictionary<string, string> vehicleAssets = AssetFinder.VehicleAssets();
            foreach (var file in vehicleAssets.Keys)
            {
                string filename;
                if (vehicleAssets.TryGetValue(file, out filename))
                {
                    GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath(filename, typeof(GameObject));

                    VehicleType vehicleType;
                    if (Enum.TryParse(file, out vehicleType))
                    {
                        _spawnableVehicles.TryAdd(vehicleType, prefab);
                    }
                }
            }
        }

        protected IEnumerator Spawn(VehicleType vehicleType, Action<GameObject> callback = null)
        {
            foreach (GameObject vehicle in _activeVehicles.Keys)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(_spawnPoint))
                {
                    Debug.Log("Cannot spawn vehicle, a vehicle exist at spawn");
                    callback?.Invoke(null);
                    yield break;
                }
            }

            GameObject vehicleAsset;
            if (_spawnableVehicles.TryGetValue(vehicleType, out vehicleAsset))
            {
                GameObject obj = Instantiate(vehicleAsset);
                obj.GetComponent<IsoTransform>().Position = _spawnPoint;
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                if (_activeVehicles.TryAdd(obj, vehicleType))
                {
                    GameObject remove;
                    _spawnableVehicles.TryRemove(vehicleType, out remove);
                    callback?.Invoke(obj);
                }
                else
                {
                    DestroyImmediate(obj);
                    callback?.Invoke(null);
                }
            }
            else
            {
                Debug.Log("Cannot spawn vehicle, this vehicle type already spawned");
            }

            Debug.Log("There are " + _activeVehicles.Count + " active vehicles");
        }

        protected IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback = null)
        {
            CustomAStarAgent customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
            yield return StartCoroutine(customAStarAgent.MoveTo(position, callback));
        }

        protected IEnumerator Destroy(Vector3 position, Action<bool> callback = null)
        {
            foreach (GameObject vehicle in _activeVehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    VehicleType b;
                    if (_activeVehicles.TryRemove(vehicle, out b))
                    {
                        yield return MoveTo(vehicle, _destroyPoint, callback);
                        DestroyImmediate(vehicle);
                        Debug.Log("There are " + _activeVehicles.Count + " active vehicles");
                        yield break;
                    }
                }
            }

            callback?.Invoke(false);
        }

        protected GameObject GetVehicleAtPosition(Vector3 position)
        {
            foreach (GameObject vehicle in _activeVehicles.Keys)
            {
                IsoTransform isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    return vehicle;
                }
            }

            return null;
        }

        protected bool AddVehicle(GameObject vehicle, VehicleType vehicleType)
        {
            bool added = _activeVehicles.TryAdd(vehicle, vehicleType);
            Debug.Log("There are " + _activeVehicles.Count + " active vehicles");
            return added;
        }

        protected VehicleType GetVehicleType(GameObject vehicle)
        {
            VehicleType type;
            _activeVehicles.TryGetValue(vehicle, out type);
            return type;
        }

        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }
    }
}