using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;

namespace LevelManager
{
    public class ListLevelManager : ArrayLevelManager
    {
        public List<IsoTransform> CarParkMedium;
        public List<IsoTransform> MediumSpawnTile;
        public List<IsoTransform> MediumDestroyTile;

        public List<IsoTransform> CarParkLarge;
        public List<IsoTransform> LargeSpawnTile;
        public List<IsoTransform> LargeDestroyTile;
        
        public List<IsoTransform> CustomMapLayout;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
