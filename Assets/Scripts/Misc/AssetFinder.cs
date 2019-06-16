using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Misc
{
    public class AssetFinder : MonoBehaviour
    {
        public static List<string> VehicleAssets()
        {
            return Directory.GetFiles("Assets/Custom/Vehicles", "*.prefab").ToList();
        }
    }
}