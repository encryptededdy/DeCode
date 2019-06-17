using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace CustomGridGraph
{
    [RequireComponent(typeof(IsoTransform))]
    public class TileRules : MonoBehaviour
    {
        public bool NW;
        public bool NE;
        public bool SW;
        public bool SE;
    }
}