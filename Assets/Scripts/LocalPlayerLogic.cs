using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	// The player who is holding the device
	public class LocalPlayerLogic : PlayerLogic
	{
		public LocalPlayerLogic(GameParam gameParam, Table table, Side playingSide, Player playerComp, HitArea hitArea) : base(gameParam, table, playingSide, playerComp)
		{
			hitArea.HitEvent += HitBall;
		}

		// Convert normalised position on Rectangle UI to another player's table-side position, then hit the ball with appropriate force
		public void HitBall(Vector2 target)
		{
			if (!playerComp.CurrentBall)
			{
				return;
			}
			Vector3 xDir = (target.x - 0.5f) * table.Width * table.Right;
			Vector3 zDir = table.Length * target.y * table.Forward;
			float ballHeightOffset = playerComp.CurrentBall.transform.position.y - table.Position.y;
			Vector3 hDir = xDir + zDir + table.Position - playerComp.CurrentBall.transform.position - (Vector3.up * ballHeightOffset);
			Vector3 vForce = (hDir.magnitude * gameParam.PlayerVForce - ballHeightOffset * gameParam.PlayerVDiffMultiplier) * Vector3.up;
			Vector3 force = hDir * gameParam.PlayerHForce + vForce;
			playerComp.HitBall(force, playingSide, true);
		}
	}
}