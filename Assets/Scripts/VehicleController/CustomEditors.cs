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

    [CustomEditor(typeof(CustomGridGraph))]
    public class CustomGridGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CustomGridGraph myScript = (CustomGridGraph) target;
            if (GUILayout.Button("Update Graph"))
            {
                myScript.UpdateGraph();
                EditorUtility.SetDirty(myScript);
            }
        }
    }
}