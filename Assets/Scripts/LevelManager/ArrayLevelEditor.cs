#if UNITY_EDITOR
using System.Collections.Generic;
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
                myScript.AddElement(_freshSpawn,
                    status => { Debug.Log(status ? "Successfully written to array" : "Failed to write to array"); });
            }

            if (GUILayout.Button("CopyFromIndexToTempVar"))
            {
                myScript.CopyFromIndexToTempVar(0,
                    status =>
                    {
                        Debug.Log(status ? "Successfully copy to temp variable" : "Failed to copy to temp variable");
                    });
            }

            if (GUILayout.Button("CopyFromTempVarToIndex"))
            {
                myScript.CopyFromTempVarToIndex(1, status =>
                {
                    Debug.Log(status
                        ? "Successfully copy from tempVar to array"
                        : "Failed to copy from tempVar to array");
                });
            }

            if (GUILayout.Button("CopyFromIndexToIndex"))
            {
                myScript.CopyFromIndexToIndex(0, 1,
                    status =>
                    {
                        Debug.Log(status
                            ? "Successfully copy from index to index"
                            : "Failed to copy from index to index");
                    });
            }

            if (GUILayout.Button("Destroy"))
            {
                myScript.Destroy(0,
                    status => { Debug.Log(status ? "Successfully destroyed a car" : "Failed to destroy a car"); });
            }

            if (GUILayout.Button("ArrayState"))
            {
                List<VehicleType> vehicleTypes = myScript.GetArrayState();
                for (var i = 0; i < vehicleTypes.Count; i++)
                {
                    Debug.Log("Vehicle at: " + i + " is " + vehicleTypes[i]);
                }
            }

            if (GUILayout.Button("Reset"))
            {
                myScript.ResetLevel(status =>
                {
                    Debug.Log(status ? "Successfully reset level" : "Failed to reset level");
                });
            }

            if (GUILayout.Button("NextStage"))
            {
                myScript.NextStage(status =>
                {
                    Debug.Log(status ? "Successfully change to next stage" : "Failed to change to next stage");
                });
            }
        }
    }
}
#endif