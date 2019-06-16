using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(ArrayLevel))]
    public class ArrayLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArrayLevel myScript = (ArrayLevel) target;
            if (GUILayout.Button("test"))
            {
                myScript.Spawn();
            }
        }
    }
}