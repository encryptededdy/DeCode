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

        public Sprite CarparkTile;
        public List<Sprite> CarparkEntranceTile;

        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;

        private static int _maxSize = 8;

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

        public static int GetMaxSize()
        {
            return _maxSize;
        }
    }
}