#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LevelManager
{
    [CustomEditor(typeof(QueueLevelManager))]
    public class ArrayLevelEditor : Editor
    {
        private Tuple<VehicleType, GameObject> _freshSpawn;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            QueueLevelManager myScript = (QueueLevelManager) target;
            if (GUILayout.Button("Spawn"))
            {
                myScript.Spawn(obj =>
                {
                    _freshSpawn = obj;
                    Debug.Log(_freshSpawn.Item1);
                });
            }

            if (GUILayout.Button("BugRepro"))
            {
                myScript.Dequeue(status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully popped");
                        myScript.Dequeue(status2 =>
                        {
                            if (status2)
                            {
                                Debug.Log("Successfully popped 2");
                                myScript.Spawn(obj =>
                                {    
                                    myScript.Enqueue(obj.Item2, status3 =>
                                    {
                                        if (status3)
                                        {
                                            Debug.Log("Successfully pushed");
                                        }
                                        else
                                        {
                                            Debug.Log("Failed to push");
                                        }
                                    });
                                });
                            }
                            else
                            {
                                Debug.Log("Failed to pop");
                            }
                        });

                    }
                    else
                    {
                        Debug.Log("Failed to pop");
                    }
                });

            }

            if (GUILayout.Button("Enqueue"))
            {
                myScript.Enqueue(_freshSpawn.Item2, status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully pushed");
                    }
                    else
                    {
                        Debug.Log("Failed to push");
                    }
                });
            }

            if (GUILayout.Button("Dequeue"))
            {
                myScript.Dequeue(status =>
                {
                    if (status)
                    {
                        Debug.Log("Successfully popped");
                    }
                    else
                    {
                        Debug.Log("Failed to pop");
                    }
                });
            }

            if (GUILayout.Button("Circular"))
            {
                myScript.SetCircularQueue(true);
            }


            if (GUILayout.Button("HideImplementation"))
            {
                myScript.SetHiddenImplementation(true);
            }

            if (GUILayout.Button("ResetLevel"))
            {
                myScript.ResetLevel(status => { });
            }
        }
    }
}
#endif