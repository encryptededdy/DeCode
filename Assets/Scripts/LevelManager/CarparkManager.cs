using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using CustomGridGraph;
using UnityEngine;

namespace LevelManager
{
    public class CarparkManager : MonoBehaviour
    {
        public List<IsoTransform> Carparks;
        public List<IsoTransform> CarparkEntrance;

        public Sprite CarparkTile;
        public List<Sprite> CarparkEntranceTile;

        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;

        private int _currentSize;
        private static int _maxSize = 8;

        public void AddCarpark()
        {
            if (_currentSize.Equals(_maxSize))
            {
                Debug.Log("Maximum carpark size reached");
            }
            else
            {
                Carparks[_currentSize].GetComponent<SpriteRenderer>().sprite = CarparkTile;
                Carparks[_currentSize].GetOrAddComponent<TileRules>().NE = true;

                CarparkEntrance[_currentSize].GetComponent<SpriteRenderer>().sprite = CarparkEntranceTile[_currentSize];
                CarparkEntrance[_currentSize].GetComponent<TileRules>().SW = true;

                _currentSize++;
                FindObjectOfType<CustomGridGraph.CustomGridGraph>().UpdateGraph();
            }
        }

        public int getSize()
        {
            return _currentSize;
        }
    }
}