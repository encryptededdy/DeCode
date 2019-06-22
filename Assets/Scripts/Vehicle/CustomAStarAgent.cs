using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using UnityEngine;

namespace Vehicle
{
    [RequireComponent(typeof(IsoTransform))]
    public class CustomAStarAgent : MonoBehaviour
    {
        public float JumpHeight = 0; //vertical distance threshold to next node
        public CustomGridGraph.CustomGridGraph Graph;
        public AstarAgent.Heuristic Heuristic;
        private CarAnimator _carAnimator;

        private void Awake()
        {
            _carAnimator = new CarAnimator(this.GetOrAddComponent<Animator>());
        }

        public IEnumerator MoveTo(Vector3 destination, int speed, Action<bool> callback = null)
        {
            var astar = new Astar(GetFromEnum(Heuristic));

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

            astar.SearchPath(startNode, endNode, JumpHeight, path =>
            {
                StopAllCoroutines();
                StartCoroutine(MoveAlongPathInternal(path, speed));
            }, () =>
            {
                Debug.Log("No path found");
                callback?.Invoke(false);
            });

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
                _carAnimator.Animate(isoTransform.Position, newPosition);
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

        private Astar.Heuristic GetFromEnum(AstarAgent.Heuristic heuristic)
        {
            switch (heuristic)
            {
                case AstarAgent.Heuristic.EuclidianDistance:
                    return Astar.EuclidianHeuristic;
                case AstarAgent.Heuristic.MaxAlongAxis:
                    return Astar.MaxAlongAxisHeuristic;
                case AstarAgent.Heuristic.ManhattenDistance:
                    return Astar.ManhattanHeuristic;
                case AstarAgent.Heuristic.AvoidVerticalSteeps:
                    return Astar.AvoidVerticalSteepsHeuristic;
                default:
                    throw new ArgumentOutOfRangeException("heuristic", heuristic, null);
            }
        }
    }
}