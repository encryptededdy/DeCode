#if UNITY_EDITOR

using System;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UltimateIsometricToolkit.physics;
using UnityEditor;
using UnityEngine;

namespace Misc
{
    [ExecuteInEditMode]
    public class BaseFloorGenerator : MonoBehaviour
    {
        // Default naming prefix
        public String Prefix = "Floor Tile";

        // Default prefab
        public GameObject Prefab;

        // Default size
        public int SizeX = 16;
        public int SizeZ = 16;
        
        void Awake()
        {
            this.GetOrAddComponent<IsoTransform>();
            this.GetOrAddComponent<IsoSorting>();
        }

        public void RePaint()
        {
            while (transform.childCount != 0)
            {
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            GenerateFloor();
        }

        private void GenerateFloor()
        {
            if (!Prefab)
            {
                Debug.Log("Select a prefab");
                return;
            }

            var isoTransform = GetComponent<IsoTransform>();
            var parentX = isoTransform.Position.x;
            var parentY = isoTransform.Position.y;
            var parentZ = isoTransform.Position.z;

            for (var dx = 0; dx <= SizeX; dx++)
            {
                for (var dz = 0; dz <= SizeZ; dz++)
                {
                    var x = parentX + dx;
                    var z = parentZ + dz;
                    Insert(Prefab, Prefix + " (" + x + ", " + z + ")", x, Prefab.GetComponent<IsoTransform>().Position.y + parentY, z);
                }
            }
        }

        private void Insert(GameObject prefab, String name, float x, float y, float z)
        {
            GameObject gameObject = Instantiate(prefab);
            gameObject.transform.parent = transform;
            gameObject.name = name;

            var isoTransform = gameObject.GetComponent<IsoTransform>();
            isoTransform.Position = new Vector3(x, y, z);
        }
    
    }
    [CustomEditor(typeof(BaseFloorGenerator))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseFloorGenerator myScript = (BaseFloorGenerator) target;
            if (GUILayout.Button("Build Object"))
            {
                myScript.RePaint();
                EditorUtility.SetDirty(myScript);
            }
        }
    }
}
#endif