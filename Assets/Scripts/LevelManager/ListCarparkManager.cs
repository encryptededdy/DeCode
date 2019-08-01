using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using CustomGridGraph;
using UnityEngine;

namespace LevelManager
{
    public class ListCarparkManager : MonoBehaviour
    {
        public List<IsoTransform> Carparks;
        public List<IsoTransform> CarparkEntrance;
        public GameObject Decorations;
        public GameObject GroundTiles;

        public Sprite CarparkTile;
        public List<Sprite> CarparkEntranceTile;

        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;

        // Currently capped at 8 due a constraint on the size of the carpark to fit on screen.
        private static int _maxSize = 8;

        /*
         * This is a method which dynamically creates a new carpark of the defined size by editing particular ground
         * tile and the gridgraph
         */
        public bool CreateCarpark(int size, out List<IsoTransform> newCarpark)
        {
            List<IsoTransform> carpark = new List<IsoTransform>();
            if (size <= _maxSize)
            {
                for (int i = 0; i < size; i++)
                {
                    Carparks[i].GetComponent<SpriteRenderer>().sprite = CarparkTile;
                    Carparks[i].GetOrAddComponent<TileRules>().NE = true;
                    carpark.Add(Carparks[i]);
                    CarparkEntrance[i].GetComponent<SpriteRenderer>().sprite = CarparkEntranceTile[i];
                    CarparkEntrance[i].GetComponent<TileRules>().SW = true;
                }
            }
            else
            {
                Debug.Log("Maximum carpark size reached");
                newCarpark = null;
                return false;
            }

            newCarpark = carpark;
            return true;
        }

        /**
         * Returns the maximum size the list can be created
         */
        public static int GetMaxSize()
        {
            return _maxSize;
        }
    }
}