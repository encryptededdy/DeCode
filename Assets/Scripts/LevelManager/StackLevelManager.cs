using System;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace LevelManager
{
    /*
     * This class is used by the stack level.
     */
    public class StackLevelManager : ADTLevelManager
    {
        /*
         * This method adds a new car into the carpark, shifts the headtile (mimic operation of a stack).
         */
        public void Push(GameObject vehicle, Action<bool> callback)
        {
            StartCoroutine(WriteToIndex(vehicle, ConvertTileToPosition(ActiveCarpark[NumElements]), callback));
            HeadTile.GetComponent<IsoTransform>().Position = ActiveCarpark[NumElements].Position;
            HeadTile.SetActive(true);
            NumElements++;
        }

        /*
         * This method pops the head element from the stack DS and shifts head element to the next (if it exist)
         */
        public void Pop(Action<bool> callback)
        {
            if (NumElements != 0)
            {
                NumElements--;
                StartCoroutine(Destroy(ConvertTileToPosition(ActiveCarpark[NumElements]), status =>
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