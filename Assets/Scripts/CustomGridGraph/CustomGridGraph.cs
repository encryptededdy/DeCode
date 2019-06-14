using System.Collections.Generic;
using System.Linq;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UnityEngine;

namespace CustomGridGraph
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

        public bool ShowGraph = false;
        private Dictionary<Vector3, Node> _gridGraph = new Dictionary<Vector3, Node>();
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
                UpdateGraphInternal(FindObjectsOfType<IsoTransform>().Where(isoT =>
                    !Ignorables.Contains(isoT) && isoT.GetComponent<TileRules>() != null));
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
                    TileRules tileRules = obj.GetComponent<TileRules>();

                    HashSet<INode> neighbours = new HashSet<INode>();
                    foreach (var adjacentCell in adjacentCells)
                    {
                        Node neighbour;
                        if (grid.TryGetValue(adjacentCell, out neighbour))
                        {
                            if (Traversable(obj.Position, neighbour.Position, tileRules))
                            {
                                neighbours.Add(neighbour);
                            }
                        }
                    }

                    Node node = new Node(obj.Position, tileRules, obj.Max.y - obj.Min.y, neighbours);

                    foreach (var neighbour in neighbours)
                    {
                        var n = (Node) neighbour;
                        if (Traversable(n.Position, node.Position, n.TileRules))
                        {
                            n.NextNodes.Add(node);
                        }
                    }

                    grid.Add(obj.Position, node);
                }
            }

            return grid;
        }


        public bool Traversable(Vector3 from, Vector3 to, TileRules tileRules)
        {
            if (from.z.Equals(to.z))
            {
                if (to.x > from.x)
                {
                    return tileRules.NE;
                }

                if (to.x < from.x)
                {
                    return tileRules.SW;
                }
            }
            else if (from.x.Equals(to.x))
            {
                if (to.z > from.z)
                {
                    return tileRules.NW;
                }

                if (to.z < from.z)
                {
                    return tileRules.SE;
                }
            }

            return false;
        }
    }
}