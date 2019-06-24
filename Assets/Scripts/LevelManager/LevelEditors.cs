#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(ListLevelManager))]
    public class ListLevelEditor : Editor
    {
        private GameObject _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ListLevelManager myScript = (ListLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { _freshSpawn = obj; });
            }

            if (GUILayout.Button("Move"))
            {
                myScript.WriteToArray(_freshSpawn, 0,
                    status => { Debug.Log(status ? "Successfully written to array" : "Failed to write to array"); });
            }

            if (GUILayout.Button("Create"))
            {
                myScript.CreateNewCarpark(8, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully created new carpark");
                    }
                    else
                    {
                        Debug.Log("Failed to create new carpark");
                    }
                });
            }
        }
    }
}
#endif