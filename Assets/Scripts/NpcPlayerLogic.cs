using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	// Holds logic for Npc player
	public class NpcPlayerLogic : PlayerLogic
	{
		private bool shouldFollow;
		private bool hasHit;

		private readonly Vector3 startPos;

		private const float SPEED = 3f;

		public NpcPlayerLogic(GameParam gameParam, Table table, Side playingSide, Player playerComp) : base(gameParam, table, playingSide, playerComp)
		{
			startPos = playerComp.Position;
		}

		// Reset behaviour when getting a new ball
		public override void OnGetBall()
		{
			playerComp.CurrentBall.CrossSideEvent += OnBallCrossSide;
			playerComp.CurrentBall.HitTableEvent += OnBallHitTable;
			hasHit = false;
			shouldFollow = false;
		}

		// When ball goes to another player's side, reset behaviour as well
		private void OnBallCrossSide(Side side)
		{
			if (side != playingSide)
			{
				shouldFollow = false;
				hasHit = false;
			}
		}

		// When the ball hit this table's side once, start following it
		private void OnBallHitTable(Side side)
		{
			if (side == playingSide && !hasHit)
			{
				shouldFollow = true;
			}
		}

		public override void Update()
		{
			if (playerComp.CurrentBall && shouldFollow)
			{
				bool isOnThisSide = playerComp.CurrentBall.CurrentSide == playingSide;
				if (isOnThisSide) // Move toward the ball when it is on this side. Double check with shouldFollow variable
				{
					Vector3 targetPos = playerComp.CurrentBall.transform.position;
					Vector3 targetOffset = targetPos - playerComp.Position;
					if (Vector3.Dot(playerComp.transform.forward, targetOffset) > 0f)
					{
						playerComp.Position = Vector3.MoveTowards(playerComp.Position, targetPos, SPEED * Time.deltaTime);
					}
				}
			}
			else // Otherwise, move toward the starting position
			{
				playerComp.Position = Vector3.MoveTowards(playerComp.Position, startPos, SPEED * Time.deltaTime);
			}
		}

		// When collider hits the ball, random the ball landing position, then hit the ball with appropriate force
		public override void OnTriggerEnter(Collider collision)
		{
			if (collision.gameObject.TryGetComponent(out Ball ball))
			{
				Vector3 xDir = Vector3.Lerp(table.LeftPos, table.RightPos, Random.Range(0f, 1f)) - table.Position;
				Vector3 zDir = Vector3.Lerp(table.Position, table.BackPos, Random.Range(0f, 1f)) - table.Position;
				float ballHeightOffset = ball.transform.position.y - table.Position.y;
				Vector3 hDir = xDir + zDir + table.Position - ball.transform.position - (Vector3.up * ballHeightOffset);
				Vector3 vForce = (hDir.magnitude * gameParam.WallVForce - ballHeightOffset * gameParam.WallVDiffMultiplier) * Vector3.up;
				Vector3 force = hDir * gameParam.WallHForce + vForce;
				playerComp.HitBall(force, playingSide, true);
				shouldFollow = false;
				hasHit = true;
			}
		}
	}
}