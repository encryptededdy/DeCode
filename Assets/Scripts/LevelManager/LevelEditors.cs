#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(ArrayLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        private GameObject _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArrayLevelManager myScript = (ArrayLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { _freshSpawn = obj; });
            }
            

//            if (GUILayout.Button("Enqueue"))
//            {
//                myScript.Enqueue(_freshSpawn, status =>
//                {
//                    if (status)
//                    {
//                        Debug.Log("Successfully Enqueued");
//                    }
//                    else
//                    {
//                        Debug.Log("Failed to Enqueue");
//                    }
//                });
//            }
//
//            if (GUILayout.Button("Dequeue"))
//            {
//                myScript.Dequeue(status =>
//                {
//                    if (status)
//                    {
//                        Debug.Log("Successfully Dequeued");
//                    }
//                    else
//                    {
//                        Debug.Log("Failed to Dequeue");
//                    }
//                });
//            }
//
//            if (GUILayout.Button("Circular"))
//            {
//                myScript.SetType(true);
//            }
//
//            if (GUILayout.Button("HideImplementation"))
//            {
//                myScript.SetHiddenImplementation(true);
//            }
        }
    }
}
#endif