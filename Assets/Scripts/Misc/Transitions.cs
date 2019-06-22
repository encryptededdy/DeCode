using System;
using System.Collections;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace Misc
{
    public static class Transitions
    {
        public static IEnumerator PanCamera(Camera camera, Vector3 shiftAmount, Action<bool> callback = null, float duration = 1f)
        {
            float t = 0.0f;
            Vector3 cameraPos = camera.GetComponent<IsoTransform>().Position;
            Vector3 startingPos = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z);

            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                camera.GetComponent<IsoTransform>().Position = Vector3.Lerp(startingPos, startingPos + shiftAmount, t);
                yield return 0;
            }

            callback?.Invoke(true);
        }
    }
}