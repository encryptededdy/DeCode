using UnityEditor;
using UnityEngine;

namespace Vehicle
{
    [CustomEditor(typeof(VehicleController))]
    public class VehicleControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VehicleController myScript = (VehicleController) target;
            if (GUILayout.Button("test"))
            {
                myScript.TestMove();
            }
        }
    }
}