using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class WallOpponent : MonoBehaviour
	{
		[SerializeField] private Transform playerT;
		[SerializeField] private GameParam gameParam;

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody.TryGetComponent(out Ball ball))
			{
				Vector3 playerOffset = playerT.position - ball.transform.position;
				Vector3 hitForceV = Vector3.up;
				Vector3 hitDirH = playerOffset - Vector3.Project(playerOffset, transform.up);
				ball.Hit((hitDirH + hitForceV).normalized, gameParam.WallForce * hitDirH.magnitude, true);
			}
		}
	}
}