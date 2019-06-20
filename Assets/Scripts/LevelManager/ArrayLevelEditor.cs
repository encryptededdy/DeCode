#if UNITY_EDITOR
using System.Collections.Generic;
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

            if (GUILayout.Button("Move"))
            {
                myScript.WriteToArray(_freshSpawn, 0,
                    status => { Debug.Log(status ? "Successfully written to array" : "Fail to write to array"); });
            }

            if (GUILayout.Button("CopyFromIndexToTempVar"))
            {
                myScript.CopyFromIndexToTempVar(0,
                    status =>
                    {
                        Debug.Log(status ? "Successfully copy to temp variable" : "Fail to copy to temp variable");
                    });
            }

            if (GUILayout.Button("CopyFromTempVarToIndex"))
            {
                myScript.CopyFromTempVarToIndex(1, status =>
                {
                    Debug.Log(status
                        ? "Successfully copy from tempVar to array"
                        : "Fail to copy from tempVar to array");
                });
            }

            if (GUILayout.Button("CopyFromIndexToIndex"))
            {
                myScript.CopyFromIndexToIndex(0, 1,
                    status =>
                    {
                        Debug.Log(status
                            ? "Successfully copy from index to index"
                            : "Fail to copy from index to index");
                    });
            }

            if (GUILayout.Button("Destroy"))
            {
                myScript.Destroy(0,
                    status => { Debug.Log(status ? "Successfully destroyed a car" : "Fail to destroy a car"); });
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
                    Debug.Log(status ? "Successfully reset level" : "Fail to reset level");
                });
            }
        }
    }
}
#endif