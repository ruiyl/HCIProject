using UnityEngine;

namespace Assets.Scripts
{
	// Hold all player logic
	public abstract class PlayerLogic
	{
		protected GameParam gameParam;
		protected Table table;
		protected Side playingSide;
		protected Player playerComp;

		protected PlayerLogic(GameParam gameParam, Table table, Side playingSide, Player playerComp)
		{
			this.gameParam = gameParam;
			this.table = table;
			this.playingSide = playingSide;
			this.playerComp = playerComp;
		}

		public virtual void Update()
		{
		}

		public virtual void OnTriggerEnter(Collider collision)
		{
		}

		public virtual void OnGetBall()
		{

		}
	}
}
