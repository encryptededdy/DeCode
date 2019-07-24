#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(StackLevelManager))]
    public class LevelEditor : Editor
    {
        private Tuple<VehicleType, GameObject> _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StackLevelManager myScript = (StackLevelManager) target;
            
            if (GUILayout.Button("StartLevel"))
            {
                myScript.StartLevel(status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully start level");
                    }
                    else
                    {
                        Debug.Log("Failed to start level");
                    }
                });
            }
            
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj =>
                {
                    _freshSpawn = obj;
                    Debug.Log(_freshSpawn.Item1);
                });
            }

            if (GUILayout.Button("Push"))
            {
                myScript.Push(_freshSpawn.Item2, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully pushed");
                    }
                    else
                    {
                        Debug.Log("Failed to push");
                    }
                });
            }

            if (GUILayout.Button("Pop"))
            {
                myScript.Pop(status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully popped");
                    }
                    else
                    {
                        Debug.Log("Failed to pop");
                    }
                });
            }

            if (GUILayout.Button("HideImplementation"))
            {
                myScript.SetHiddenImplementation(true);
            }

            if (GUILayout.Button("ResetLevel"))
            {
                myScript.ResetLevel(status => { });
            }
        }
    }
}
#endif