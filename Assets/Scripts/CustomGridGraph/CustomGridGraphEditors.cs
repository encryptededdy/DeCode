using UnityEditor;
using UnityEngine;

namespace CustomGridGraph
{
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