using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	// Provide MonoBehaviour features for player logic class
	public class Player : MonoBehaviour
	{
		private PlayerLogic logic;
		private Ball currentBall;
		private bool isInit;

		public Ball CurrentBall { get => currentBall; }

		public Func<Side, bool> RequestBallHitting;

		private void Update()
		{
			logic?.Update();
		}

		private void OnTriggerEnter(Collider other)
		{
			logic?.OnTriggerEnter(other);
		}

		public void SetCurrentBall(Ball ball)
		{
			currentBall = ball;
			logic.OnGetBall();
		}

		// Check if can hit the ball before actually hitting it
		public void HitBall(Vector3 force, Side hitter, bool clearVelocity)
		{
			if (RequestBallHitting?.Invoke(hitter) ?? false)
			{
				currentBall.Hit(force, hitter, clearVelocity);
			}
		}

		// Create Npc logic
		public void InitAsNpc(GameParam gameParam, Table table, Side playingSide)
		{
			if (isInit)
			{
				Debug.LogError("Already Initalised");
				return;
			}
			logic = new NpcPlayerLogic(gameParam, table, playingSide, this);
			isInit = true;
		}

		// Create Local player logic
		public void InitAsLocalPlayer(GameParam gameParam, Table table, Side playingSide, HitArea hitArea)
		{
			if (isInit)
			{
				Debug.LogError("Already Initalised");
				return;
			}
			logic = new LocalPlayerLogic(gameParam, table, playingSide, this, hitArea);
			isInit = true;
		}

		public Vector3 Position
		{
			get
			{
				return transform.position;
			}
			set
			{
				transform.position = value;
			}
		}
	}
}