using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{
	public enum Side
	{
		None = 0,
		Player = 1,
		NPC = 2,
	}

	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Camera cam;
		[SerializeField] private Transform[] points;
		[SerializeField] private Transform boundaryT;
		[SerializeField] private Ball ballPrefab;
		[SerializeField] private GameParam gameParam;
		[SerializeField] private WallOpponent wall;
		[SerializeField] private HitArea hitArea;
		[SerializeField] private TextMeshProUGUI scorePlayerT;
		[SerializeField] private TextMeshProUGUI scoreNpcT;
		[SerializeField] private TextMeshProUGUI announcementT;

		private Ball ball;
		private Table table;
		private int playerScore;
		private int npcScore;
		private Side lastTableSideHit;
		private Side lastHitter;
		private float announcementEnd;

		public Table GameTable { get => table; }

		private const float ANNOUNCEMENT_DURATION = 1f;
		private const string PLAYER_FOUL = "Player's Foul!";

		private void Awake()
		{
			table = new Table(points[0], points[1], points[2], points[3]);
			wall.SetReferences(table);
			hitArea.HitEvent += HitBall;
		}

		private void Update()
		{
			if (ball && ball.BallState == Ball.State.Play && !IsBallInside(ball.transform.position, boundaryT))
			{
				GiveScore(lastTableSideHit == Side.None ? Side.Player : lastTableSideHit);
				SpawnBall();
			}
			if (Time.time > announcementEnd)
			{
				announcementT.gameObject.SetActive(false);
			}
		}

		public void Found()
		{
			gameObject.SetActive(true);
			SpawnBall();
		}

		public void Lost()
		{
			gameObject.SetActive(false);
		}

		public void SpawnBall()
		{
			if (ball)
			{
				Destroy(ball.gameObject);
			}
			ball = Instantiate(ballPrefab);
			ball.SetStartingState(cam.transform, table);
			ball.HitEvent += OnBallHit;
			ball.HitTableSide1Event += OnHitTableSide1;
			ball.HitTableSide2Event += OnHitTableSide2;
			ball.HitWallEvent += OnHitWall;
			ball.BallStopEvent += OnBallStop;
			lastTableSideHit = Side.None;
			lastHitter = Side.None;
		}

		public bool IsBallInside(Vector3 ballPos, Transform boundary)
		{
			Vector3 ballOffset = ballPos - boundary.position;
			float xOffset = Vector3.Dot(ballOffset, boundary.right);
			float yOffset = Vector3.Dot(ballPos, boundary.up);
			return (Mathf.Abs(xOffset) <= boundary.localScale.x / 2f) && (Mathf.Abs(yOffset) <= boundary.localScale.y / 2f);
		}

		public void HitBall(Vector2 target)
		{
			if (lastTableSideHit == Side.NPC || ball.CurrentSide == Side.NPC)
			{
				OnPlayerFoul();
				return;
			}
			Vector3 xDir = (target.x - 0.5f) * table.Width * table.Right;
			Vector3 zDir = table.Length * target.y * table.Forward;
			float ballHeightOffset = ball.transform.position.y - table.Position.y;
			Vector3 hDir = xDir + zDir + table.Position - ball.transform.position - (Vector3.up * ballHeightOffset);
			Vector3 vForce = (hDir.magnitude * gameParam.PlayerVForce - ballHeightOffset * gameParam.PlayerVDiffMultiplier) * Vector3.up;
			Vector3 force = hDir * gameParam.PlayerHForce + vForce;
			ball.Hit(force, Side.Player, true);
		}

		private void OnBallHit(Side hitter)
		{
			if (lastHitter == hitter)
			{
				if (hitter == Side.Player)
				{
					OnPlayerFoul();
				}
				else
				{
					GiveScore(hitter);
					SpawnBall();
				}
			}
			else
			{
				lastHitter = hitter;
			}
		}

		private void OnHitWall()
		{
			if (lastTableSideHit == Side.Player)
			{
				OnPlayerFoul();
			}
		}

		private void OnHitTableSide1()
		{
			OnHitTable(Side.Player);
		}

		private void OnHitTableSide2()
		{
			if (lastTableSideHit == Side.None)
			{
				OnPlayerFoul();
				return;
			}
			OnHitTable(Side.NPC);
		}

		private void OnHitTable(Side side)
		{
			if (lastTableSideHit == side)
			{
				GiveScore(side);
				SpawnBall();
			}
			else
			{
				lastTableSideHit = side;
			}
		}

		private void OnBallStop()
		{
			GiveScore(lastTableSideHit);
		}

		private void GiveScore(Side lostSide)
		{
			TextMeshProUGUI text = null;
			int score = 0;
			string scoreS = "";
			switch (lostSide)
			{
				case Side.NPC:
					text = scorePlayerT;
					score = ++playerScore;
					scoreS = "YOU: {0}";
					break;
				case Side.Player:
					text = scoreNpcT;
					score = ++npcScore;
					scoreS = "NPC: {0}";
					break;
				default:
					return;
			}
			text.text = string.Format(scoreS, score);
		}

		private void OnPlayerFoul()
		{
			GiveScore(Side.Player);
			SpawnBall();
			AddAnnoucement(PLAYER_FOUL);
		}

		private void AddAnnoucement(string text)
		{
			announcementT.text = text;
			announcementT.gameObject.SetActive(true);
			announcementEnd = Time.time + ANNOUNCEMENT_DURATION;
		}
	}

	public struct Table
	{
		public Vector3 Position { get; private set; }
		public Vector3 NetTop { get; private set; }
		public Vector3 LeftPos { get; private set; }
		public Vector3 RightPos { get; private set; }
		public Vector3 FrontPos { get; private set; }
		public Vector3 BackPos { get; private set; }
		public Vector3 Right { get; private set; }
		public Vector3 Forward { get; private set; }
		public float Width { get; private set; }
		public float Length { get; private set; }
		public float NetHeight { get; private set; }

		public Table(Transform point1, Transform point2, Transform point3, Transform point4)
		{
			Position = point1.position;
			NetTop = point4.position;
			NetHeight = (point4.position - point1.position).magnitude;
			LeftPos = point3.position;
			Vector3 left = point3.position - point1.position;
			Right = -left.normalized;
			Width = 2f * left.magnitude;
			RightPos = point1.position - left;
			Vector3 back = point2.position - point1.position;
			Forward = -back.normalized;
			Length = 2f * back.magnitude;
			BackPos = point2.position;
			FrontPos = point1.position - back;
		}
	}
}