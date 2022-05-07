using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
	public class Ball : MonoBehaviour, IPointerDownHandler
	{
		[SerializeField] private LayerMask side1Mask;
		[SerializeField] private LayerMask side2Mask;
		[SerializeField] private LayerMask netMask;
		[SerializeField] private LayerMask wallMask;

		private Rigidbody rb;
		private Transform cameraT;
		private Table table;
		private GameObject contactedObj;
		private float contactTimer;
		private Side currentSide;

		private State state;

		public State BallState { get => state; }
		public Side CurrentSide { get => currentSide; }

		public UnityAction<PointerEventData> PointerDownEvent;
		public UnityAction<Side> HitEvent;
		public UnityAction<Side> HitTableEvent;
		public UnityAction HitNetEvent;
		public UnityAction HitWallEvent;
		public UnityAction BallStopEvent;
		public UnityAction<Side> CrossSideEvent;

		private const float BALL_STOP_TRESHOLD = 0.5f;

		public enum State
		{
			Start,
			Play,
		}

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			PointerDownEvent?.Invoke(eventData);
		}

		public void Hit(Vector3 force, Side hitter, bool clearVelocity)
		{
			if (state == State.Start)
			{
				state = State.Play;
				rb.isKinematic = false;
				rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
			if (clearVelocity)
			{
				rb.velocity = Vector3.zero;
			}
			rb.AddForce(force, ForceMode.VelocityChange);
			HitEvent?.Invoke(hitter);
		}

		private void Update()
		{
			switch (state)
			{
				case State.Start:
					transform.position = cameraT.position + (cameraT.forward * 0.2f);
					transform.rotation = cameraT.rotation;
					break;
				case State.Play:
					if (contactedObj)
					{
						contactTimer += Time.deltaTime;
						if (contactTimer > BALL_STOP_TRESHOLD)
						{
							BallStopEvent?.Invoke();
						}
					}
					Side side = Vector3.Dot(transform.position - table.Position, table.Forward) >= 0f ? Side.Away : Side.Home;
					if (currentSide != side)
					{
						currentSide = side;
						CrossSideEvent?.Invoke(currentSide);
					}
					break;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			LayerMask otherL = 1 << collision.gameObject.layer;
			if (otherL == side1Mask)
			{
				HitTableEvent?.Invoke(Side.Home);
			}
			else if (otherL == side2Mask)
			{
				HitTableEvent?.Invoke(Side.Away);
			}
			else if (otherL == netMask)
			{
				HitNetEvent?.Invoke();
			}
			else if (otherL == wallMask)
			{
				HitWallEvent?.Invoke();
			}
			contactedObj = collision.gameObject;
			contactTimer = 0f;
		}

		private void OnCollisionExit(Collision collision)
		{
			contactedObj = null;
		}

		public void SetStartingState(Transform cameraT, Table table)
		{
			state = State.Start;
			currentSide = Side.Home;
			this.cameraT = cameraT;
			rb.isKinematic = true;
			this.table = table;
		}
	}
}