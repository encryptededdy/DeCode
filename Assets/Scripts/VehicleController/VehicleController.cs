using UnityEngine;

namespace VehicleController
{
    public class VehicleController : MonoBehaviour
    {
        private CustomAStarAgent _astarAgent;


        // Start is called before the first frame update
        void Awake()
        {
            _astarAgent = this.GetOrAddComponent<CustomAStarAgent>();
        }

        public void test()
        {
            _astarAgent.MoveTo(new Vector3(5, 1, 4));
        }
    }
}