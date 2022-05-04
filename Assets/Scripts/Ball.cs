using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Ball : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private GameParam gameParam;

        private Rigidbody rb;
        private Transform cameraT;

        private State state;

        public State BallState { get => state; }

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
            HitByPlayer();
        }

        private void HitByPlayer()
        {
            Hit(cameraT.forward);
        }

        public void Hit(Vector3 forceDirection)
        {
            if (Vector3.Distance(rb.position, cameraT.position) > 0.3f)
            {
                return;
            }
            if (state == State.Start)
            {
                state = State.Play;
                rb.isKinematic = false;
            }
            rb.velocity = Vector3.zero;
            rb.AddForce(forceDirection * gameParam.Force, ForceMode.VelocityChange);
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

        public void SetStartingState(Transform cameraT)
        {
            state = State.Start;
            this.cameraT = cameraT;
            rb.isKinematic = true;
        }
    }
}