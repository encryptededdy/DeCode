using System.Collections.Concurrent;
using UnityEngine;

namespace Vehicle
{
    public class VehicleClones : MonoBehaviour
    {
        private ConcurrentQueue<GameObject> clones;
        // Start is called before the first frame update

        public VehicleClones()
        {
            clones = new ConcurrentQueue<GameObject>(); 
        }
        
        public void AddClone(GameObject clone)
        {
            clones.Enqueue(clone);
        }
        public GameObject NextClone()
        {
            GameObject clone;
            clones.TryDequeue(out clone);
            return clone;
        }

        public bool HasClone()
        {
            return !clones.IsEmpty;
        }
    }
}
