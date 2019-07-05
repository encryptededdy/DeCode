using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace Misc
{
    public class TransitionManager : MonoBehaviour
    {
        public IEnumerator PanCameraEffect(Camera sceneCamera, Vector3 shiftAmount, Action<bool> callback = null,
            float duration = 0.5f)
        {
            float t = 0.0f;
            Vector3 cameraPos = sceneCamera.GetComponent<IsoTransform>().Position;
            Vector3 startingPos = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z);

            while (t < 1.0f)
            {
                t += Time.deltaTime * (Time.timeScale / duration);
                sceneCamera.GetComponent<IsoTransform>().Position =
                    Vector3.Lerp(startingPos, startingPos + shiftAmount, t);
                yield return 0;
            }

            Debug.Log("Pan Camera");
            callback?.Invoke(true);
        }

        public IEnumerator SpawnCarparkEffect(GameObject carpark, GameObject decorations, Action<bool> callback = null,
            int tilesPerIteration = 8)
        {
            if (decorations != null)
            {
                foreach (Transform child in decorations.transform)
                {
                    Material material = child.GetComponent<SpriteRenderer>().material;
                    var color = material.color;
                    color.a = 0f;
                    material.color = color;
                }
            }
            
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

            for (int i = 0; i < shuffledList.Count; i = i + tilesPerIteration)
            {
                int completed = 0;
                int expected = tilesPerIteration;
                for (int j = 0; j < tilesPerIteration; j++)
                {
                    if (i + j < shuffledList.Count)
                    {
                        StartCoroutine(FadeAnimation(shuffledList[i + j], FadeDirection.In, true, status =>
                        {
                            if (status)
                            {
                                completed++;
                            }
                        }));
                    }
                    else
                    {
                        expected--;
                    }
                }

                yield return new WaitUntil(() => completed == expected);
            }

            if (decorations != null)
            {
                foreach (Transform child in decorations.transform)
                {
                    StartCoroutine(FadeAnimation(child.gameObject, FadeDirection.In, false, null, 1f));
                }
            }

            callback?.Invoke(true);
            Debug.Log("Created a new carpark");
        }

        public IEnumerator DestroyCarparkEffect(GameObject carpark, GameObject decorations,
            Action<bool> callback = null, int tilesPerIteration = 8)
        {
            callback?.Invoke(true);

            foreach (Transform child in decorations.transform)
            {
                StartCoroutine(FadeAnimation(child.gameObject, FadeDirection.Out, false, null, 1f));
            }

            List<GameObject> tiles = new List<GameObject>();
            foreach (Transform child in carpark.transform)
            {
                tiles.Add(child.gameObject);
            }

            List<GameObject> shuffledList = Randomiser.ShuffleList(tiles);

            for (int i = 0; i < shuffledList.Count; i = i + tilesPerIteration)
            {
                int completed = 0;
                int expected = tilesPerIteration;
                for (int j = 0; j < tilesPerIteration; j++)
                {
                    if (i + j < shuffledList.Count)
                    {
                        StartCoroutine(FadeAnimation(shuffledList[i + j], FadeDirection.Out, true, status =>
                        {
                            if (status)
                            {
                                completed++;
                            }
                        }));
                    }
                    else
                    {
                        expected--;
                    }
                }

                yield return new WaitUntil(() => completed == expected);
            }

            callback?.Invoke(true);
            Debug.Log("Destroyed old carpark");
        }

        private IEnumerator FadeAnimation(GameObject tile,
            FadeDirection fadeDirection,
            bool move,
            Action<bool> callback = null,
            float duration = 0.15f
        )
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

                if (move)
                {
                    tile.GetComponent<IsoTransform>().Position = Vector3.Lerp(startingPos, endingPos, t);
                }

                yield return 0;
            }

            callback?.Invoke(true);
        }

        private enum FadeDirection
        {
            In,
            Out
        }
    }
}