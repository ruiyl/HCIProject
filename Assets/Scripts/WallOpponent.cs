using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class WallOpponent : MonoBehaviour
	{
		[SerializeField] private GameParam gameParam;

		private Table table;
		private Vector3 startDebugLine;
		private Vector3 endDebugLine;

		public void SetReferences(Table table)
		{
			this.table = table;
		}

		private void Update()
		{
			Debug.DrawLine(startDebugLine, endDebugLine, Color.red, Time.deltaTime);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody.TryGetComponent(out Ball ball))
			{
				startDebugLine = ball.transform.position;
				Vector3 xDir = Vector3.Lerp(table.LeftPos, table.RightPos, Random.Range(0f, 1f)) - table.Position;
				Vector3 zDir = Vector3.Lerp(table.Position, table.BackPos, Random.Range(0f, 1f)) - table.Position;
				float ballHeightOffset = ball.transform.position.y - table.Position.y;
				Vector3 hDir = xDir + zDir + table.Position - ball.transform.position - (Vector3.up * ballHeightOffset);
				Vector3 vForce = (hDir.magnitude * gameParam.WallVForce - ballHeightOffset * gameParam.WallVDiffMultiplier) * Vector3.up;
				Vector3 force = hDir * gameParam.WallHForce + vForce;
				ball.Hit(force, Side.NPC, true);
				endDebugLine = force + ball.transform.position;
			}
		}
	}
}