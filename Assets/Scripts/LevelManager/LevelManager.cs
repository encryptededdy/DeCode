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
        public List<GameObject> CustomVehicleAssets;
        public List<IsoTransform> ActiveCarpark;
        public IsoTransform ActiveSpawnTile;
        public IsoTransform ActiveDestroyTile;
        public GameObject Carpark;
        public GameObject Decorations;
        protected CustomGridGraph.CustomGridGraph GridGraph;
        private Vector3 _spawnPoint;
        private Vector3 _destroyPoint;
        protected TransitionManager TransitionManager;

        // Only using the key as we want thread-safe DSes with ability to lookup in O(1).
        private ConcurrentDictionary<GameObject, VehicleType> _activeVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _spawnableVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _customSpawnableVehicles;

        protected abstract void OnAwake();
        
        void Awake()
        {
            TransitionManager = gameObject.AddComponent<TransitionManager>();
            _activeVehicles = new ConcurrentDictionary<GameObject, VehicleType>();
            LoadAssets();
            OnAwake();
        }

        protected IEnumerator Spawn(VehicleType vehicleType, Action<Tuple<VehicleType, GameObject>> callback = null)
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
                obj.GetComponent<IsoTransform>().Position = _spawnPoint;
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                if (_activeVehicles.TryAdd(obj, vehicleType))
                {
                    _spawnableVehicles.TryRemove(vehicleType, out _);
                    callback?.Invoke(new Tuple<VehicleType, GameObject>(vehicleType, obj));
                }
                else
                {
                    DestroyImmediate(obj);
                    callback?.Invoke(null);
                }
            }
            else if (_customSpawnableVehicles.TryGetValue(vehicleType, out vehicleAsset))
            {
                GameObject obj = Instantiate(vehicleAsset);
                obj.GetComponent<IsoTransform>().Position = _spawnPoint;
                obj.GetComponent<CustomAStarAgent>().Graph = FindObjectOfType<CustomGridGraph.CustomGridGraph>();
                if (_activeVehicles.TryAdd(obj, vehicleType))
                {
                    _customSpawnableVehicles.TryRemove(vehicleType, out _);
                    callback?.Invoke(new Tuple<VehicleType, GameObject>(vehicleType, obj));
                }
                else
                {
                    DestroyImmediate(obj);
                    callback?.Invoke(null);
                }
            }
            else
            {
                callback?.Invoke(null);
                Debug.Log("Cannot spawn vehicle, this vehicle type already spawned");
            }

            Debug.Log($"There are {_activeVehicles.Count} active vehicles");
        }

        protected IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback, bool fast = false)
        {
            var customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();
            
            vehicle.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            
            if (fast)
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 10, status =>
                {
                    vehicle.GetComponent<SpriteRenderer>().color = new Color(0.745283f, 0.745283f, 0.745283f);
                    callback(status);
                }));
            }
            else
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 3, status =>
                {
                    vehicle.GetComponent<SpriteRenderer>().color = new Color(0.745283f, 0.745283f, 0.745283f);
                    callback(status);
                }));
            }
        }

        protected IEnumerator Destroy(Vector3 position, Action<bool> callback, bool fast = false)
        {
            foreach (var vehicle in _activeVehicles.Keys)
            {
                var isoTransform = vehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    yield return MoveTo(vehicle, _destroyPoint, callback, fast);
                    if (_activeVehicles.TryRemove(vehicle, out VehicleType type))
                    {
                        DestroyImmediate(vehicle);
                        ResetVehicle(type);
                        Debug.Log($"There are {_activeVehicles.Count} active vehicles");
                        yield break;
                    }
                }
            }

            callback?.Invoke(false);
        }

        protected IEnumerator WriteToIndex(GameObject clone, Vector3 carpark, Action<bool> callback, bool fast = false)
        {
            int completed = 0;
            int expected = 2;
            if (!GetVehicleAtPosition(carpark, out _))
            {
                Debug.Log("No need to overwrite");
                expected--;
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

            if (clone.GetComponent<IsoTransform>().Position.Equals(carpark))
            {
                Debug.Log("Vehicle already at destination");
                expected--;
            }
            else
            {
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
            }

            yield return new WaitUntil(() => completed == expected);
            callback?.Invoke(true);
        }

        protected bool GetVehicleAtPosition(Vector3 position, out GameObject vehicle)
        {
            foreach (var activeVehicle in _activeVehicles.Keys)
            {
                var isoTransform = activeVehicle.GetComponent<IsoTransform>();
                if (isoTransform.Position.Equals(position))
                {
                    vehicle = activeVehicle;
                    return true;
                }
            }

            vehicle = null;
            return false;
        }

        protected bool AddVehicle(GameObject vehicle, VehicleType vehicleType)
        {
            bool added = _activeVehicles.TryAdd(vehicle, vehicleType);
            Debug.Log($"There are {_activeVehicles.Count} active vehicles");
            return added;
        }

        protected VehicleType GetVehicleType(GameObject vehicle)
        {
            _activeVehicles.TryGetValue(vehicle, out VehicleType type);
            return type;
        }

        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }

        private void LoadAssets()
        {
            _spawnableVehicles = new ConcurrentDictionary<VehicleType, GameObject>();
            _customSpawnableVehicles = new ConcurrentDictionary<VehicleType, GameObject>();

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

            foreach (var asset in CustomVehicleAssets)
            {
                if (Enum.TryParse(asset.name, out VehicleType vehicleType))
                {
                    if (_customSpawnableVehicles.TryAdd(vehicleType, asset))
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
                            return;
                        }
                    }
                }
            }

            foreach (var asset in CustomVehicleAssets)
            {
                if (Enum.TryParse(asset.name, out VehicleType type))
                {
                    if (vehicleType.Equals(type))
                    {
                        if (_customSpawnableVehicles.TryAdd(vehicleType, asset))
                        {
                            Debug.Log($"Added Vehicle {asset.name}");
                            return;
                        }
                    }
                }
            }
        }

        public void ResetLevel(Action<bool> callback, bool fast = false)
        {
            StartCoroutine(RemoveAllVehicles(callback, fast));
        }

        protected IEnumerator RemoveAllVehicles(Action<bool> callback, bool fast = false)
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
            _spawnPoint = ConvertTileToPosition(spawnTile);
        }

        protected void SetNewDestroyPoint(IsoTransform destroyTile)
        {
            _destroyPoint = ConvertTileToPosition(destroyTile);
        }
    }
}