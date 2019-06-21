using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Misc;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    public abstract class LevelManager : MonoBehaviour
    {
        public List<GameObject> VehicleAssets;
        public List<IsoTransform> ActiveCarpark;
        public IsoTransform ActiveSpawnTile;
        public IsoTransform ActiveDestroyTile;
        protected Vector3 SpawnPoint;
        protected Vector3 DestroyPoint;

        // Only using the key as we want thread-safe DSes with ability to lookup in O(1).
        private ConcurrentDictionary<GameObject, VehicleType> _activeVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _spawnableVehicles;

        void Awake()
        {
            _activeVehicles = new ConcurrentDictionary<GameObject, VehicleType>();
            SetNewSpawnPoint(ActiveSpawnTile);
            SetNewDestroyPoint(ActiveDestroyTile);
            LoadAssets();
        }

        protected IEnumerator Spawn(VehicleType vehicleType, Action<GameObject> callback = null)
        {
            foreach (var vehicle in _activeVehicles.Keys)
            {
                if (vehicle.GetComponent<IsoTransform>().Position.Equals(SpawnPoint))
                {
                    Debug.Log("Cannot spawn vehicle, a vehicle exist at spawn");
                    callback?.Invoke(null);
                    yield break;
                }
            }

            if (vehicleType == VehicleType.random)
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
                obj.GetComponent<IsoTransform>().Position = SpawnPoint;
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

        private IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback, bool fast = false)
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

        protected IEnumerator Destroy(Vector3 position, Action<bool> callback, bool fast = false)
        {
            foreach (var vehicle in _activeVehicles.Keys)
            {
                var isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    yield return MoveTo(vehicle, DestroyPoint, callback, fast);
                    if (_activeVehicles.TryRemove(vehicle, out VehicleType type))
                    {
                        DestroyImmediate(vehicle);
                        ResetVehicle(type);
                        Debug.Log("There are " + _activeVehicles.Count + " active vehicles");
                        yield break;
                    }
                }
            }

            callback?.Invoke(false);
        }

        protected IEnumerator WriteToIndex(GameObject clone, Vector3 carpark, Action<bool> callback, bool fast = false)
        {
            int completed = 0;
            if (GetVehicleAtPosition(carpark) == null)
            {
                Debug.Log("No need to overwrite");
                completed++;
            }
            else
            {
                StartCoroutine(Destroy(carpark, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully overwritten index");
                        completed++;
                    }
                    else
                    {
                        Debug.Log("Failed to overwrite index");
                        callback?.Invoke(false);
                    }
                }, fast));
            }

            StartCoroutine(MoveTo(clone, carpark, status =>
            {
                if (status)
                {
                    Debug.Log("Successfully move vehicle to index");
                    completed++;
                }
                else
                {
                    Debug.Log("Failed to move vehicle to index");
                    callback?.Invoke(false);
                }
            }, fast));

            yield return new WaitUntil(() => completed == 2);
            callback?.Invoke(true);
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

        private void ResetVehicle(VehicleType vehicleType)
        {
            foreach (VehicleType vehicle in _activeVehicles.Values)
            {
                if (vehicle.Equals(vehicleType))
                {
                    return;
                }
            }

            foreach (var asset in VehicleAssets)
            {
                if (Enum.TryParse(asset.name, out VehicleType type))
                {
                    if (vehicleType.Equals(type))
                    {
                        if (_spawnableVehicles.TryAdd(vehicleType, asset))
                        {
                            Debug.Log($"Added Vehicle {asset.name}");
                        }
                    }
                }
            }
        }

        protected IEnumerator ResetLevel(Action<bool> callback, bool fast = false)
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
                }, fast));
            }

            yield return new WaitUntil(() => _activeVehicles.IsEmpty);
            LoadAssets();
            callback?.Invoke(true);
        }

        protected void SetNewSpawnPoint(IsoTransform spawnTile)
        {
            SpawnPoint = ConvertTileToPosition(spawnTile);
        }

        protected void SetNewDestroyPoint(IsoTransform destroyTile)
        {
            DestroyPoint = ConvertTileToPosition(destroyTile);
        }

        protected void SetNewActiveCarpark(List<IsoTransform> carpark)
        {
            ActiveCarpark = carpark;
        }
    }
}