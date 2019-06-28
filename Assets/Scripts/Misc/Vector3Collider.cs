using System;
using UnityEngine;

namespace Misc
{
    public static class Vector3Collider
    {
        public static bool Intersect(Vector3 obj1, Vector3 obj2, Vector3 tile, float tolerance = 0.5f)
        {
            return Math.Abs(obj1.x - obj2.x) < tile.x * tolerance && Math.Abs(obj1.y - obj2.y) < tile.y * tolerance &&
                   Math.Abs(obj1.z - obj2.z) < tile.z * tolerance;
        }
    }
}