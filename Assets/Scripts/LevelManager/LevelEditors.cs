using UnityEditor;
using UnityEngine;
using Vehicle;

namespace LevelManager
{
    [CustomEditor(typeof(ArrayLevel))]
    public class ArrayLevelEditor : Editor
    {
        private GameObject obj;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ArrayLevel myScript = (ArrayLevel) target;
            if (GUILayout.Button("Spawn"))
            {
                 obj = myScript.Spawn();
            }
            
            if (GUILayout.Button("Move"))
            {
                CustomAStarAgent AStarAgent = obj.GetComponent<CustomAStarAgent>();
                AStarAgent.MoveTo(new Vector3(2, 0, 4));
            }
            
            if (GUILayout.Button("Destroy"))
            {
                myScript.Destroy(0);
            }
        }
    }
}