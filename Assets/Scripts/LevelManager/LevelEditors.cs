using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(ArrayLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArrayLevelManager myScript = (ArrayLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.WriteToArray(myScript.Spawn(), 0);
            }

            if (GUILayout.Button("Destroy"))
            {
                myScript.Destroy(0);
            }
        }
    }
}