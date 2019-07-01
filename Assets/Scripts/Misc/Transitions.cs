using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace Misc
{
    public class Transitions
    {
        public static IEnumerator PanCamera(Camera camera, Vector3 shiftAmount, Action<bool> callback = null,
            float duration = 1f)
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

        public static IEnumerator SpawnCarparkEffect(GameObject carpark)
        {
            List<GameObject> tiles = new List<GameObject>();
            foreach (Transform child in carpark.transform)
            {
                tiles.Add(child.gameObject);
                Material material = child.GetComponent<SpriteRenderer>().material;
                var color = material.color;
                color.a = 0f;
                material.color = color;
            }

            List<GameObject> shuffledList = Randomiser.ShuffleList(tiles);
            foreach (GameObject child in shuffledList)
            {
                yield return FadeAnimation(child, FadeDirection.In);
            }

            Debug.Log("Created a new carpark");
        }

        public static IEnumerator DestroyCarparkEffect(GameObject carpark)
        {
            List<GameObject> tiles = new List<GameObject>();
            foreach (Transform child in carpark.transform)
            {
                tiles.Add(child.gameObject);
            }

            List<GameObject> shuffledList = Randomiser.ShuffleList(tiles);
            foreach (GameObject child in shuffledList)
            {
                yield return FadeAnimation(child, FadeDirection.Out);
            }

            Debug.Log("Destroyed old carpark");
        }

        private static IEnumerator FadeAnimation(GameObject tile, FadeDirection fadeDirection,
            Action<bool> callback = null,
            float duration = 0.025f)
        {
            float t = 0f;

            Material material = tile.GetComponent<SpriteRenderer>().material;
            Color color = material.color;

            Vector3 position = tile.GetComponent<IsoTransform>().Position;
            Vector3 startingPos;
            Vector3 endingPos;
            float startingOpacity;
            float endingOpacity;
            if (fadeDirection.Equals(FadeDirection.In))
            {
                startingPos = new Vector3(position.x, position.y - 0.4f, position.z);
                endingPos = new Vector3(position.x, position.y, position.z);
                startingOpacity = 0.1f;
                endingOpacity = 1f;
            }
            else
            {
                startingPos = new Vector3(position.x, position.y, position.z);
                endingPos = new Vector3(position.x, position.y - 0.4f, position.z);
                startingOpacity = 1f;
                endingOpacity = 0.1f;
            }

            while (t < 1.0f)
            {
                // Step the fade forward one frame.
                t += Time.deltaTime * (Time.timeScale / duration);

                float blend = Mathf.Clamp01(t / duration);

                // Blend to the corresponding opacity between start & target.
                color.a = Mathf.Lerp(startingOpacity, endingOpacity, blend);
                material.color = color;

                tile.GetComponent<IsoTransform>().Position = Vector3.Lerp(startingPos, endingPos, t);

                yield return 0;
            }

            callback?.Invoke(true);
        }

        public enum FadeDirection
        {
            In,
            Out
        }
    }
}