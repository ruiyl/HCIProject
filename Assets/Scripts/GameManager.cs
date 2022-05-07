using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Scripts
{
	public enum Side
	{
		None = 0,
		Home = 1,
		Away = 2,
	}

	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Camera cam;
		[SerializeField] private Transform[] points;
		[SerializeField] private Transform boundaryT;
		[SerializeField] private Ball ballPrefab;
		[SerializeField] private GameParam gameParam;
		[SerializeField] private Player homePlayer;
		[SerializeField] private Player awayPlayer;
		[SerializeField] private HitArea hitArea;
		[SerializeField] private TextMeshProUGUI scorePlayerT;
		[SerializeField] private TextMeshProUGUI scoreNpcT;
		[SerializeField] private TextMeshProUGUI announcementT;

		private Ball ball;
		private Table table;
		private int playerScore;
		private int opponentScore;
		private Side lastTableSideHit;
		private Side lastHitter;
		private float announcementEnd;
		private Dictionary<Side, Player> players;

		public Table GameTable { get => table; }

		private const float ANNOUNCEMENT_DURATION = 1f;
		private const string READY = "READY!";
		private const string PLAYER_SCORE = "PLAYER: {0}";
		private const string PLAYER_GET = "Player got one!";
		private const string OPPONENT_SCORE = "OPPONENT: {0}";
		private const string PLAYER_FOUL = "Player's Foul!";
		private const string OPPONENT_FOUL = "Opponent's Foul!";
		private const string OPPONENT_GET = "Opponent got one!";

		private void Awake()
		{
			table = new Table(points[0], points[1], points[2], points[3]);
			players = new Dictionary<Side, Player>()
			{
				[Side.Home] = homePlayer,
				[Side.Away] = awayPlayer,
			};
			homePlayer.InitAsLocalPlayer(gameParam, table, Side.Home, hitArea);
			awayPlayer.InitAsNpc(gameParam, table, Side.Away);
			foreach (Player p in players.Values)
			{
				p.RequestBallHitting = OnPlayerPreHitBall;
			}
		}

		private void Start()
		{
			SpawnBall();
		}

		private void Update()
		{
			if (ball && ball.BallState == Ball.State.Play && !IsBallInside(ball.transform.position, boundaryT))
			{
				LoseScore(lastTableSideHit == Side.None ? lastHitter : lastTableSideHit);
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
			StartCoroutine(SpawnBallTask());
		}

		private IEnumerator SpawnBallTask()
		{
			if (announcementEnd < float.MaxValue)
			{
				yield return new WaitUntil(() => Time.time >= announcementEnd);
			}
			AddAnnoucement(READY, ANNOUNCEMENT_DURATION);
			yield return new WaitForSeconds(ANNOUNCEMENT_DURATION);
			ball = Instantiate(ballPrefab);
			ball.SetStartingState(cam.transform, table);
			ball.HitEvent += OnBallHit;
			ball.HitTableEvent += OnBallHitTable;
			ball.BallStopEvent += OnBallStop;
			lastTableSideHit = Side.None;
			lastHitter = Side.None;

			foreach (Player p in players.Values)
			{
				p.SetCurrentBall(ball);
			}
		}

		public bool IsBallInside(Vector3 ballPos, Transform boundary)
		{
			Vector3 ballOffset = ballPos - boundary.position;
			float xOffset = Vector3.Dot(ballOffset, boundary.right);
			float yOffset = Vector3.Dot(ballPos, boundary.up);
			return (Mathf.Abs(xOffset) <= boundary.localScale.x / 2f) && (Mathf.Abs(yOffset) <= boundary.localScale.y / 2f);
		}

		private bool OnPlayerPreHitBall(Side hitter)
		{
			if (lastTableSideHit == Side.None && lastHitter == Side.None)
			{
				return true;
			}
			if (lastHitter == hitter)
			{
				OnPlayerFoul(hitter);
				Debug.Log("Foul 1");
				return false;
			}
			if (lastTableSideHit != hitter)
			{
				OnPlayerFoul(hitter);
				Debug.Log("Foul 2");
				return false;
			}
			if (ball.CurrentSide != hitter)
			{
				OnPlayerFoul(hitter);
				Debug.Log("Foul 3");
				return false;
			}
			return true;
		}

		private void OnBallHit(Side hitter)
		{
			lastHitter = hitter;
		}

		private void OnBallHitTable(Side tableSideHit)
		{
			if (lastTableSideHit == tableSideHit)
			{
				LoseScore(tableSideHit);
				SpawnBall();
			}
			else
			{
				if (lastTableSideHit == Side.None && lastHitter != tableSideHit)
				{
					OnPlayerFoul(lastHitter);
					Debug.Log("Foul 4");
				}
				else
				{
					lastTableSideHit = tableSideHit;
				}
			}
		}

		private void OnBallStop()
		{
			LoseScore(lastTableSideHit);
		}

		private void LoseScore(Side lostSide)
		{
			TextMeshProUGUI text;
			int score;
			string scoreS;
			switch (lostSide)
			{
				case Side.Away:
					text = scorePlayerT;
					score = ++playerScore;
					scoreS = PLAYER_SCORE;
					AddAnnoucement(PLAYER_GET, ANNOUNCEMENT_DURATION);
					break;
				case Side.Home:
					text = scoreNpcT;
					score = ++opponentScore;
					scoreS = OPPONENT_SCORE;
					AddAnnoucement(OPPONENT_GET, ANNOUNCEMENT_DURATION);
					break;
				default:
					return;
			}
			text.text = string.Format(scoreS, score);
		}

		private void OnPlayerFoul(Side foulSide)
		{
			LoseScore(foulSide);
			SpawnBall();
			switch (foulSide)
			{
				case Side.None:
					Debug.LogError("Invalid side");
					break;
				case Side.Home:
					AddAnnoucement(PLAYER_FOUL, ANNOUNCEMENT_DURATION);
					break;
				case Side.Away:
					AddAnnoucement(OPPONENT_FOUL,ANNOUNCEMENT_DURATION);
					break;
			}
		}

		private void AddAnnoucement(string text, float duration)
		{
			announcementT.text = text;
			announcementT.gameObject.SetActive(true);
			if (duration > 0f)
			{
				announcementEnd = Time.time + duration;
			}
			else
			{
				announcementEnd = float.MaxValue;
			}
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