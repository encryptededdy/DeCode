#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(StackLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        private GameObject _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StackLevelManager myScript = (StackLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { _freshSpawn = obj; });
            }

            if (GUILayout.Button("Push"))
            {
                myScript.Push(_freshSpawn, status =>
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
        }
    }
}
#endif