#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    /*
     * This class is used for quick testing via buttons in the editor
     */
    [CustomEditor(typeof(StackLevelManager))]
    public class LevelEditor : Editor
    {
        private Tuple<VehicleType, GameObject> _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StackLevelManager myScript = (StackLevelManager) target;

//            if (GUILayout.Button("StartLevel"))
//            {
//                myScript.StartLevel(status =>
//                {
//                    if (status)
//                    {
//                        Debug.Log("Successfully start level");
//                    }
//                    else
//                    {
//                        Debug.Log("Failed to start level");
//                    }
//                });
//            }

//            if (GUILayout.Button("Spawn"))
//            {
//                myScript.Spawn(obj =>
//                {
//                    _freshSpawn = obj;
//                    Debug.Log(_freshSpawn.Item1);
//                });
//            }
        }
    }
}
#endif