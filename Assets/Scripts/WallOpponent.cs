using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class WallOpponent : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody.TryGetComponent(out Ball ball))
            {
                ball.Hit(transform.forward);
            }
        }
    }
}