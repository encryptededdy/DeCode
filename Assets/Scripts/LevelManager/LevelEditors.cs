#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(QueueLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        private GameObject _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            QueueLevelManager myScript = (QueueLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { _freshSpawn = obj; });
            }

            if (GUILayout.Button("Dequeue"))
            {
                myScript.Enqueue(_freshSpawn, status =>
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

            if (GUILayout.Button("Enqueue"))
            {
                myScript.Dequeue(status =>
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

            if (GUILayout.Button("Circular"))
            {
                myScript.SetCircularQueue(true);
            }


            if (GUILayout.Button("HideImplementation"))
            {
                myScript.SetHiddenImplementation(true);
            }
        }
    }
}
#endif