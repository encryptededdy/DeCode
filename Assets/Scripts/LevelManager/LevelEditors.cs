using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(ArrayLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        private GameObject freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArrayLevelManager myScript = (ArrayLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { freshSpawn = obj; });
            }

            if (GUILayout.Button("Move"))
            {
                myScript.WriteToArray(freshSpawn, 0, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully written to array");
                    }
                    else
                    {
                        Debug.Log("Fail to write to array");
                    }
                });
            }

            if (GUILayout.Button("Copy"))
            {
                myScript.CopyToTempVariable(freshSpawn, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully copy to temp variable");
                    }
                    else
                    {
                        Debug.Log("Fail to copy to temp variable");
                    }
                });
            }

            if (GUILayout.Button("Destroy"))
            {
                myScript.Destroy(0, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully destroyed a car");
                    }
                    else
                    {
                        Debug.Log("Fail to destroy a car");
                    }
                });
            }
        }
    }
}