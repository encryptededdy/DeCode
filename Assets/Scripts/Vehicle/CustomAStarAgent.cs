using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using UnityEngine;

namespace Vehicle
{
    /**
     * This is used for most of the pathfinding behaviour (of the vehicles). Should not need to go into details of this.
     * This is customised version extended from the IsoToolKit.
     */
    [RequireComponent(typeof(IsoTransform))]
    public class CustomAStarAgent : MonoBehaviour
    {
        public float JumpHeight = 0; //vertical distance threshold to next node
        public CustomGridGraph.CustomGridGraph Graph;
        public AstarAgent.Heuristic Heuristic;
        private CustomVehicleAnimator _customVehicleAnimator;

        private void Awake()
        {
            _customVehicleAnimator = gameObject.GetComponent<CustomVehicleAnimator>();
        }

        public IEnumerator MoveTo(Vector3 destination, int speed, Action<bool> callback = null)
        {
            var astar = new CustomAStar(GetFromEnum(Heuristic));

            var startNode = Graph.ClosestNode(GetComponent<IsoTransform>().Position);
            var endNode = Graph.ClosestNode(destination);
            if (startNode == null)
            {
                Debug.Log("Invalid position, no node found close enough to " +
                          GetComponent<IsoTransform>().Position);
                callback?.Invoke(false);
                yield break;
            }

            if (endNode == null)
            {
                Debug.Log("Invalid position, no node found close enough to " + destination);
                callback?.Invoke(false);
                yield break;
            }

            Debug.Log("Searching Path");
            yield return StartCoroutine(astar.SearchPath(startNode, endNode, JumpHeight, path =>
            {
                StopAllCoroutines();
                Debug.Log("Found new Path, trying to move");
                StartCoroutine(MoveAlongPathInternal(path, speed));
            }, () =>
            {
                Debug.Log("No path found");
                callback?.Invoke(false);
            }));
    
            yield return new WaitUntil(() => gameObject.GetComponent<IsoTransform>().Position.Equals(destination));
            callback?.Invoke(true);
        }

        private IEnumerator StepTo(Vector3 from, Vector3 to, float speed)
        {
            var timePassed = 0f;
            var isoTransform = GetComponent<IsoTransform>();
            var maxTimePassed = Vector3.Distance(from, to) / speed;

            while (isoTransform.Position != to)
            {
                timePassed += Time.deltaTime;

                Vector3 newPosition = Vector3.Lerp(from, to, timePassed / maxTimePassed);
                _customVehicleAnimator.Animate(isoTransform.Position, newPosition);
                isoTransform.Position = new Vector3(newPosition.x, isoTransform.Position.y, newPosition.z);

                // If time has passed then round coordinates to nearest node.
                if (timePassed >= maxTimePassed)
                {
                    var pos = isoTransform.Position;
                    pos = new Vector3(Mathf.Round(pos.x), pos.y, Mathf.Round(pos.z));
                    isoTransform.Position = pos;
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator MoveAlongPathInternal(IEnumerable<Vector3> path, int speed)
        {
            foreach (var pos in path)
            {
                yield return StepTo(GetComponent<IsoTransform>().Position,
                    pos + new Vector3(0, GetComponent<IsoTransform>().Size.y / 2, 0), speed);
            }
        }

        private CustomAStar.Heuristic GetFromEnum(AstarAgent.Heuristic heuristic)
        {
            switch (heuristic)
            {
                case AstarAgent.Heuristic.EuclidianDistance:
                    return CustomAStar.EuclidianHeuristic;
                case AstarAgent.Heuristic.MaxAlongAxis:
                    return CustomAStar.MaxAlongAxisHeuristic;
                case AstarAgent.Heuristic.ManhattenDistance:
                    return CustomAStar.ManhattanHeuristic;
                case AstarAgent.Heuristic.AvoidVerticalSteeps:
                    return CustomAStar.AvoidVerticalSteepsHeuristic;
                default:
                    throw new ArgumentOutOfRangeException("heuristic", heuristic, null);
            }
        }
    }
}