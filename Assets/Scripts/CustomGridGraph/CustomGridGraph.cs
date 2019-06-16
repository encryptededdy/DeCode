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
                UpdateGraphInternal(FindObjectsOfType<TileRules>().Where(obj => obj.GetComponent<IsoTransform>() != null));
        }

        private Dictionary<Vector3, Node> UpdateGraphInternal(IEnumerable<TileRules> allTiles)
        {
            var grid = new Dictionary<Vector3, Node>();

            foreach (var tile in allTiles)
            {
                IsoTransform isoTransform = tile.GetComponent<IsoTransform>();
                if (!grid.ContainsKey(isoTransform.Position))
                {
                    var adjacentCells = AdjacentPositions.Select(adjacentPosition => adjacentPosition + isoTransform.Position)
                        .ToArray();

                    HashSet<INode> neighbours = new HashSet<INode>();
                    foreach (var adjacentCell in adjacentCells)
                    {
                        Node neighbour;
                        if (grid.TryGetValue(adjacentCell, out neighbour))
                        {
                            if (Traversable(isoTransform.Position, neighbour.Position, tile))
                            {
                                neighbours.Add(neighbour);
                            }
                        }
                    }

                    Node node = new Node(isoTransform.Position, tile, isoTransform.Max.y - isoTransform.Min.y, neighbours);

                    foreach (var neighbour in neighbours)
                    {
                        var n = (Node) neighbour;
                        if (Traversable(n.Position, node.Position, n.TileRules))
                        {
                            n.NextNodes.Add(node);
                        }
                    }

                    grid.Add(isoTransform.Position, node);
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