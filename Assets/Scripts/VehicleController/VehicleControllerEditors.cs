using UnityEditor;
using UnityEngine;

namespace VehicleController
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
                myScript.test();
            }
        }
    }
}