#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(QueueLevelManager))]
    public class ListLevelEditor : Editor
    {
        private GameObject _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            QueueLevelManager myScript = (QueueLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj => { _freshSpawn = obj; }, VehicleType.ambulance);
            }

            if (GUILayout.Button("Enqueue"))
            {
                myScript.Enqueue(_freshSpawn, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully Enqueued");
                    }
                    else
                    {
                        Debug.Log("Failed to Enqueue");
                    }
                });
            }
        }
    }
}
#endif