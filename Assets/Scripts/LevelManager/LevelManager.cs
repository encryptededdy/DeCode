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

        private ConcurrentDictionary<GameObject, VehicleType> _activeVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _spawnableVehicles;
        private ConcurrentDictionary<VehicleType, GameObject> _customSpawnableVehicles;

        protected abstract void OnAwake();

        /*
         * This method is called as the scene is loaded (used for setup), dynamically loads all vehicle assets.
         */
        void Awake()
        {
            TransitionManager = gameObject.AddComponent<TransitionManager>();
            _activeVehicles = new ConcurrentDictionary<GameObject, VehicleType>();
            LoadAssets();
            OnAwake();
        }

        /*
         * This method contains custom spawn animation for starting the level. Once completed, the callback will return
         * true else if there are errors, it returns false.
         */
        public void StartLevel(Action<bool> callback)
        {
            Carpark.SetActive(true);
            Decorations.SetActive(true);
            StartCoroutine(TransitionManager.SpawnCarparkEffect(Carpark, Decorations,
                status =>
                {
                    if (status)
                    {
                        // Need to update graph after the tiles are correctly loaded (as they move while the animation
                        // is running.
                        FindObjectOfType<CustomGridGraph.CustomGridGraph>().UpdateGraph();
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                }));
        }

        /*
         * This method is used to reset the level by remove all cars from the carpark
         */
        public void ResetLevel(Action<bool> callback, bool fast = false)
        {
            StartCoroutine(RemoveAllVehicles(callback, fast));
        }

        /*
         * This method is used to spawn a car at the defined spawn point. This is a coroutine which provides a callback
         * so the animation can be queued up.
         *
         * If the input vehicleType is VehicleType.random then it spawns a random vehicle out of all spawnable one (ones
         * that are not in the carpark). Otherwise, it will spawned the specified VehicleType if it is not already spawned.
         *
         * The separate _customSpawnableVehicles list is for managing custom assets (e.g. garbage A->E)
         */
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

            // Getting a random vehicle which is spawnable
            if (vehicleType == VehicleType.random)
            {
                if (!Randomiser.RandomValuesFromDict(_spawnableVehicles, out vehicleType))
                {
                    Debug.Log("No vehicle left to spawn");
                    callback?.Invoke(null);
                }
            }

            // Try to spawn the defined vehicleType (from spawnable list first then custom list)
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

        /*
         * This method is used to move a car to the specified position using A* pathfinding (via defined
         * GridGraph on the level). This is a coroutine which provides a callback so the animation can be queued up.
         *
         * The fast parameter is used to speedup the animation.
         */
        protected IEnumerator MoveTo(GameObject vehicle, Vector3 position, Action<bool> callback, bool fast = false)
        {
            vehicle.GetComponent<CustomVehicleAnimator>().SetAsPointOfInterest(true);
            var customAStarAgent = vehicle.GetComponent<CustomAStarAgent>();

            if (fast)
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 10, callback));
            }
            else
            {
                yield return StartCoroutine(customAStarAgent.MoveTo(position, 3, callback));
            }
            
            vehicle.GetComponent<CustomVehicleAnimator>().SetAsPointOfInterest(false);
        }

        /*
         * This method is used to remove car from the carpark, the car move to the destroy point before the gameobject
         * is destroyed from the scene. This is a coroutine which provides a callback so the animation can be queued up.
         *
         * The parameter position is used to check if a car exist at that position.
         */
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

        /*
         * This method is used to move the cloned vehicle from original position to the destination carpark. If a
         * car exists at the destination, then that car is removed from the carpark (equivalent to losing reference to
         * the variable). This is a coroutine which provides a callback so the animation can be queued up.
         * 
         */
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

        /*
         * This method is used to get the vehicle from the specified position (if it exists)
         */
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

        /*
         * This method is used to add vehicle to the list of tracked vehicles once spawned or cloned.
         */
        protected bool AddVehicle(GameObject vehicle, VehicleType vehicleType)
        {
            bool added = _activeVehicles.TryAdd(vehicle, vehicleType);
            Debug.Log($"There are {_activeVehicles.Count} active vehicles");
            return added;
        }

        /*
         * This method is used to extract thev VehicleType for the specified vehicle
         */
        protected VehicleType GetVehicleType(GameObject vehicle)
        {
            _activeVehicles.TryGetValue(vehicle, out VehicleType type);
            return type;
        }

        /*
         * This is used to convert the position of the tile to the position of the tile in the gridgraph (usually
         * directly above the tile)
         */
        protected static Vector3 ConvertTileToPosition(IsoTransform tile)
        {
            return new Vector3(tile.Position.x, tile.Position.y + tile.Size.y, tile.Position.z);
        }

        /*
         * This method is used to reset the list of spawnable vehicles so that it can be added again (done by reloading
         * from dynamic assets)
         */
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

        /*
        * This method is used to dynamically load vehicles into spawnable and customSpawnable vehicle lists
        * so they can be spawned again.
        */
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

        /*
         * Resetting the level by removing all cars from the carpark and re-loading assets (so that it can spawned
         * again)
         */
        private IEnumerator RemoveAllVehicles(Action<bool> callback, bool fast = false)
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

        /**
         * Used for setting spawn point
         */
        protected void SetNewSpawnPoint(IsoTransform spawnTile)
        {
            _spawnPoint = ConvertTileToPosition(spawnTile);
        }

        /**
         * Used for setting destroy point
         */
        protected void SetNewDestroyPoint(IsoTransform destroyTile)
        {
            _destroyPoint = ConvertTileToPosition(destroyTile);
        }
    }
}