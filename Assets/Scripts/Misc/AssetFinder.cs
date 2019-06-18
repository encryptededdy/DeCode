﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Misc
{
    public class AssetFinder : MonoBehaviour
    {
        // Returns Dictionary<FileNameWithoutExtension, FullPathNameForLoading>
        public static Dictionary<string, string> VehicleAssets()
        {
            Dictionary<string, string> vehicleAssets = new Dictionary<string, string>();
            foreach (string filename in Directory.GetFiles("Assets/Custom/Vehicles", "*.prefab"))
            {
                vehicleAssets.Add(Path.GetFileNameWithoutExtension(new FileInfo(filename).Name), filename);
            }

            return vehicleAssets;
        }
    }
}