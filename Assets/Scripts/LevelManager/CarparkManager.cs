using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class CarparkManager : MonoBehaviour
    {
        public List<IsoTransform> Carparks;
        public List<IsoTransform> CarparkEntrance;

        public Sprite CarparkTile;
        public Sprite CarparkEntranceTile;

        public IsoTransform SpawnTile;
        public IsoTransform DestroyTile;

        private int _currentSize;
        private static int _maxSize = 8;

        public bool AddCarpark()
        {
            if (_currentSize.Equals(_maxSize))
            {
                Debug.Log("Maximum carpark size reached");
                return false;
            }
            Carparks[_currentSize].GetComponent<SpriteRenderer>().sprite = CarparkTile;
            CarparkEntrance[_currentSize].GetComponent<SpriteRenderer>().sprite = CarparkEntranceTile;
            
            _currentSize++;
            FindObjectOfType<CustomGridGraph.CustomGridGraph>().UpdateGraph();
            return true;
        }

    }
}
