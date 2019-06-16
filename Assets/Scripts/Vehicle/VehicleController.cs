using UnityEngine;

namespace Vehicle
{
    public class VehicleController : MonoBehaviour
    {
        private CustomAStarAgent _astarAgent;

        // Start is called before the first frame update
        void Awake()
        {
            _astarAgent = this.GetOrAddComponent<CustomAStarAgent>();
        }

        public void TestMove()
        {
            _astarAgent.MoveTo(new Vector3(2, 0, 4));
        }
    }
}