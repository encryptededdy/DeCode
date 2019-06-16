using UnityEngine;

namespace Vehicle
{
    public class CarAnimator
    {
        private Animator _animator;

        public CarAnimator(Animator animator)
        {
            _animator = animator;
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

        private void ResetDirectionFlags()
        {
            _animator.SetBool("NE", false);
            _animator.SetBool("SW", false);
            _animator.SetBool("SE", false);
            _animator.SetBool("NW", false);
        }
    }
}