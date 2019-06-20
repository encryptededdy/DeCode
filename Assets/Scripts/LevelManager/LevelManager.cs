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
        public List<GameObject> VehicleAssets;
        
        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;
        private Vector3 _spawnPoint;
        private Vector3 _destroyPoint;
        
        // Only using the key as we want a thread-safe DS with ability to lookup in O(1).
        private ConcurrentDictionary<GameObject, VehicleType> _activeVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _spawnableVehicles;

        protected LevelManager()
        {
            _activeVehicles = new ConcurrentDictionary<GameObject, VehicleType>();
        }

        void Awake()
        {
            _spawnPoint = ConvertTileToPosition(SpawnTile);
            _destroyPoint = ConvertTileToPosition(DestroyTile);
            LoadAssets();
        }

        protected IEnumerator Spawn(VehicleType vehicleType, Action<GameObject> callback = null)
        {
            foreach (var vehicle in _activeVehicles.Keys)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(_spawnPoint))
                {
                    Debug.Log("Cannot spawn vehicle, a vehicle exist at spawn");
                    callback?.Invoke(null);
                    yield break;
                }
            }

            if (vehicleType == VehicleType.empty)
            {
                if (!Randomiser.RandomValuesFromDict(_spawnableVehicles, out vehicleType))
                {
                    Debug.Log("No vehicle left to spawn");
                    callback?.Invoke(null);
                }
            }

            if (_spawnableVehicles.TryGetValue(vehicleType, out var vehicleAsset))
            {
                GameObject obj = Instantiate(vehicleAsset);
                obj.GetComponent<IsoTransform>().Position = _spawnPoint;
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                if (_activeVehicles.TryAdd(obj, vehicleType))
                {
                    _spawnableVehicles.TryRemove(vehicleType, out _);
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

        protected IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback = null, bool fast = false)
        {
            var customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
            if (fast)
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 10, callback));
            }
            else
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 3, callback));
            }
        }

        protected IEnumerator Destroy(Vector3 position, Action<bool> callback = null)
        {
            foreach (var vehicle in _activeVehicles.Keys)
            {
                var isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    yield return MoveTo(vehicle, _destroyPoint, callback);
                    if (_activeVehicles.TryRemove(vehicle, out _))
                    {
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
            foreach (var vehicle in _activeVehicles.Keys)
            {
                var isoTransform = vehicle.GetComponent<IsoTransform>();
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
            _activeVehicles.TryGetValue(vehicle, out var type);
            return type;
        }

        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }

        private void LoadAssets()
        {
            _spawnableVehicles = new ConcurrentDictionary<VehicleType, GameObject>();
            foreach (var asset in VehicleAssets)
            {
                if (Enum.TryParse(asset.name, out VehicleType vehicleType))
                {
                    if (_spawnableVehicles.TryAdd(vehicleType, asset))
                    {
                        Debug.Log($"Added Vehicle {asset.name}");
                    }
                }
            }
        }

        protected IEnumerator ResetLevel(Action<bool> callback)
        {
            foreach (GameObject activeVehicle in _activeVehicles.Keys)
            {
                StartCoroutine(Destroy(activeVehicle.GetComponent<IsoTransform>().Position, status =>
                {
                    if (!status)
                    {
                        Debug.Log("Failed to destroy vehicles, please restart level");
                        callback?.Invoke(false);
                    }
                }));
            }

            yield return new WaitUntil(() => _activeVehicles.IsEmpty);
            LoadAssets();
            callback?.Invoke(true);
        }
    }
}