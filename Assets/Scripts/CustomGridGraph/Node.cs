using System;
using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.Pathfinding;
using UnityEngine;

namespace CustomGridGraph
{
    public class Node : INode, IEquatable<Node>
    {
        public Vector3 Position { get; }

        public TileRules TileRules { get; }

        public HashSet<INode> NextNodes { get; private set; }

        public bool Passable { get; set; }

        public float Height { get; }

        public Node(Vector3 position, TileRules tileRules, float height, HashSet<INode> neighbours)
        {
            Position = position + new Vector3(0, height, 0);
            TileRules = tileRules;
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
            return other != null && Position.Equals(other.Position);
        }
    }
}