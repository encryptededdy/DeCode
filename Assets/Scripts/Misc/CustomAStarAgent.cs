using System;
using System.Collections;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using UnityEngine;
using static Assets.UltimateIsometricToolkit.Scripts.Pathfinding.AstarAgent;

namespace Misc
{
    [RequireComponent(typeof(IsoTransform))]
    public class CustomAStarAgent : MonoBehaviour
    {
        public float JumpHeight = 0; //vertical distance threshold to next node
        public float Speed = 2; //units per second
        public CustomGridGraph Graph;
        public Heuristic heuristic;
        private CarAnimator _carAnimator;

        private void Awake()
        {
            _carAnimator = new CarAnimator(this.GetOrAddComponent<Animator>());
        }

        public void MoveTo(Vector3 destination)
        {
            var astar = new Astar(GetFromEnum(heuristic));

            var startNode = Graph.ClosestNode(GetComponent<IsoTransform>().Position);
            var endNode = Graph.ClosestNode(destination);
            if (startNode == null)
            {
                Debug.LogError("Invalid position, no node found close enough to " +
                               GetComponent<IsoTransform>().Position);
                return;
            }

            if (endNode == null)
            {
                Debug.LogError("Invalid position, no node found close enough to " + destination);
                return;
            }

            astar.SearchPath(startNode, endNode, JumpHeight, path =>
            {
                StopAllCoroutines();
                StartCoroutine(MoveAlongPathInternal(path));
            }, () => { Debug.Log("No path found"); });
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
                    pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    isoTransform.Position = pos;
                    break;
                }

                yield return null;
            }
        }

        private IEnumerator MoveAlongPathInternal(IEnumerable<Vector3> path)
        {
            foreach (var pos in path)
            {
                yield return StepTo(GetComponent<IsoTransform>().Position,
                    pos + new Vector3(0, GetComponent<IsoTransform>().Size.y / 2, 0), Speed);
            }
        }

        private Astar.Heuristic GetFromEnum(Heuristic heuristic)
        {
            switch (heuristic)
            {
                case Heuristic.EuclidianDistance:
                    return Astar.EuclidianHeuristic;
                case Heuristic.MaxAlongAxis:
                    return Astar.MaxAlongAxisHeuristic;
                case Heuristic.ManhattenDistance:
                    return Astar.ManhattanHeuristic;
                case Heuristic.AvoidVerticalSteeps:
                    return Astar.AvoidVerticalSteepsHeuristic;
                default:
                    throw new ArgumentOutOfRangeException("heuristic", heuristic, null);
            }
        }
    }
}