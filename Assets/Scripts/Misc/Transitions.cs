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
        
        public static IEnumerator FadeIn(GameObject tile, float startOpacity, float targetOpacity, float duration = 1f) {

            // Cache the current color of the material, and its initiql opacity.
            Material material = tile.GetComponent<SpriteRenderer>().material;
            Color color = material.color;


            Vector3 position = tile.GetComponent<IsoTransform>().Position;
            Vector3 startingPos = new Vector3(position.x, position.y - 0.4f, position.z);                 
            Vector3 endinggPos = new Vector3(position.x, position.y, position.z);
            // Track how many seconds we've been fading.
            float t = 0;

            while(t < duration) {
                // Step the fade forward one frame.
                t += Time.deltaTime;

                float blend = Mathf.Clamp01(t / duration);

                // Blend to the corresponding opacity between start & target.
                color.a = Mathf.Lerp(startOpacity, targetOpacity, blend);
                material.color = color;
                
                tile.GetComponent<IsoTransform>().Position = Vector3.Lerp(startingPos, endinggPos, t);
                
                yield return 0;
            }
        }
    }
}