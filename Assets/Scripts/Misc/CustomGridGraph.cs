using System;
using System.Collections.Generic;
using System.Linq;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Misc
{
    public class CustomGridGraph : MonoBehaviour
    {
        private static readonly Vector3 NorthEast = new Vector3(1, 0, 0);
        private static readonly Vector3 NorthWest = new Vector3(0, 0, 1);
        private static readonly Vector3 SouthEast = new Vector3(-1, 0, 0);
        private static readonly Vector3 SouthWest = new Vector3(0, 0, -1);

        private static readonly Vector3[] AdjacentPositions =
        {
            NorthEast, SouthEast, SouthWest, NorthWest
        };

        public float MaxScanHeight = 20;
        public bool ShowGraph = false;
        public Dictionary<Vector3, Node> _gridGraph = new Dictionary<Vector3, Node>();
        public List<IsoTransform> Ignorables = new List<IsoTransform>();

        #region Unity Callbacks 

        void Start()
        {
            UpdateGraph();
        }

        void OnDrawGizmos()
        {
            if (!ShowGraph)
                return;
            Gizmos.color = Color.red;
            foreach (var gridPos in _gridGraph.Keys)
            {
                var node = _gridGraph[gridPos];
                Gizmos.color = node.Passable ? Color.red : Color.black;
                var center = new Vector3(node.Position.x, node.Position.y, node.Position.z);
                var size = new Vector3(1, node.Height, 1);
                GizmosExtension.DrawIsoWireCube(center, size);
                foreach (var nextNode in node.NextNodes)
                {
                    GizmosExtension.DrawIsoArrow(node.Position, nextNode.Position);
                }
            }
        }

        #endregion

        public Node ClosestNode(Vector3 position)
        {
            return _gridGraph.Select(kvp => kvp.Value).OrderBy(n => (position - n.Position).sqrMagnitude)
                .FirstOrDefault();
        }


        public void UpdateGraph()
        {
            _gridGraph =
                UpdateGraphInternal(FindObjectsOfType<IsoTransform>().Where(isoT => !Ignorables.Contains(isoT)));
        }


        private Dictionary<Vector3, Node> UpdateGraphInternal(IEnumerable<IsoTransform> worldObjects)
        {
            var grid = new Dictionary<Vector3, Node>();

            foreach (var obj in worldObjects)
            {
                if (!grid.ContainsKey(obj.Position))
                {
                    var adjacentCells = AdjacentPositions.Select(adjacentPosition => adjacentPosition + obj.Position)
                        .ToArray();

                    HashSet<INode> neighbours = new HashSet<INode>();
                    foreach (var adjacentCell in adjacentCells)
                    {
                        Node neighbour;
                        if (grid.TryGetValue(adjacentCell, out neighbour))
                        {
                            neighbours.Add(neighbour);
                        }
                    }

                    Node node = new Node(obj.Position, obj.Max.y - obj.Min.y, neighbours);

                    foreach (var neighbour in neighbours)
                    {
                        neighbour.NextNodes.Add(node);
                    }

                    grid.Add(obj.Position, node);
                }
            }

            return grid;
        }

        public class Node : INode, IEquatable<Node>
        {
            public Vector3 Position { get; }

            public HashSet<INode> NextNodes { get; private set; }

            public bool Passable { get; set; }

            public float Height { get; }

            public Node(Vector3 position, float height, HashSet<INode> neighbours)
            {
                Position = position + new Vector3(0, height, 0);
                Height = height;
                NextNodes = neighbours;
                Passable = true;
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }

            public bool Equals(Node other)
            {
                return Position.Equals(other.Position);
            }
        }
    }

    [CustomEditor(typeof(CustomGridGraph))]
    public class CustomGridGraphEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CustomGridGraph myScript = (CustomGridGraph) target;
            if (GUILayout.Button("Update Graph"))
            {
                myScript.UpdateGraph();
                EditorUtility.SetDirty(myScript);
            }
        }
    }
}