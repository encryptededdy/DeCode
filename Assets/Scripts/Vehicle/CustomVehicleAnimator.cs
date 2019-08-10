using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

namespace Vehicle
{
    /*
     * This car is used to change the direction the car is facing with the animator as well as setting an arrow above
     * the vehicle as a PointOfInterest indicator.
     */
    public class CustomVehicleAnimator : MonoBehaviour
    {
        public GameObject PointOfInterestIndicator;
        private Animator _animator;

        private void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void Animate(Vector3 from, Vector3 to)
        {
            ResetDirectionFlags();
            if (from.z.Equals(to.z))
            {
                if (to.x > from.x)
                {
                    _animator.SetBool("NE", true);
                }

                if (to.x < from.x)
                {
                    _animator.SetBool("SW", true);
                }
            }
            else if (from.x.Equals(to.x))
            {
                if (to.z > from.z)
                {
                    _animator.SetBool("NW", true);
                }

                if (to.z < from.z)
                {
                    _animator.SetBool("SE", true);
                }
            }
        }

        public void SetAsPointOfInterest(bool pointOfInterest)
        {
            PointOfInterestIndicator.SetActive(pointOfInterest);
        }

        private void ResetDirectionFlags()
        {
            _animator.SetBool("NE", false);
            _animator.SetBool("SW", false);
            _animator.SetBool("SE", false);
            _animator.SetBool("NW", false);
        }
    }
}