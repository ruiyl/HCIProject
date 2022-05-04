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

		private Rigidbody rb;
		private Transform cameraT;

		private State state;

		public State BallState { get => state; }

		public UnityAction<PointerEventData> PointerDownEvent;
		public UnityAction HitSide1Event;
		public UnityAction HitSide2Event;
		public UnityAction HitNetEvent;

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

		public void Hit(Vector3 forceDirection, float force, bool clearVelocity)
		{
			if (state == State.Start)
			{
				state = State.Play;
				rb.isKinematic = false;
			}
			if (clearVelocity)
			{
				rb.velocity = Vector3.zero;
			}
			rb.AddForce(forceDirection * force, ForceMode.VelocityChange);
			Debug.Log(forceDirection);
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
					
					break;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			LayerMask otherL = collision.collider.gameObject.layer;
			if (otherL == side1Mask)
			{
				HitSide1Event?.Invoke();
			}
			else if (otherL == side2Mask)
			{
				HitSide2Event?.Invoke();
			}
			else if (otherL == netMask)
			{
				HitNetEvent?.Invoke();
			}
		}

		public void SetStartingState(Transform cameraT)
		{
			state = State.Start;
			this.cameraT = cameraT;
			rb.isKinematic = true;
		}
	}
}