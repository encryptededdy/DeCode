using System;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    public class StackLevelManager : ADTLevelManager
    {
        public void Push(GameObject vehicle, Action<bool> callback)
        {
            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(ActiveCarpark[NumElements]), callback));
            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[NumElements].Position;
            HeadTile.SetActive(true);
            NumElements++;
        }

        public void Pop(Action<bool> callback)
        {
            if (NumElements != 0)
            {
                NumElements--;
                StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[NumElements - 1]), status =>
                {
                    if (status)
                    {
                        if (NumElements == 0)
                        {
                            HeadTile.SetActive(false);
                        }
                        else
                        {
                            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[NumElements - 1].Position;
                        }

                        callback(true);
                    }
                    else
                    {
                        Debug.Log("Failed to pop vehicle");
                        callback(false);
                    }
                }));
            }
            else
            {
                Debug.Log("Stack is empty");
                callback(true);
            }
        }
    }
}