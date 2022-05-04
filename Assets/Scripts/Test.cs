using UnityEngine;

namespace Assets.Scripts
{
    public class Test : MonoBehaviour
	{
		[SerializeField] private Camera cam;
        [SerializeField] private Transform tableT;
        [SerializeField] private Transform boundaryT;
        [SerializeField] private Ball ballPrefab;

        private Ball ball;

        private void Update()
        {
            if (ball && ball.BallState == Ball.State.Play && !IsBallInside(ball.transform.position, boundaryT))
            {
                SpawnBall();
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
            ball.SetStartingState(cam.transform);
        }

        public bool IsBallInside(Vector3 ballPos, Transform boundary)
        {
            Vector3 ballOffset = ballPos - boundary.position;
            float xOffset = Vector3.Dot(ballOffset, boundary.right);
            //float zOffset = Vector3.Dot(ballOffset, boundary.forward);
            float yOffset = Vector3.Dot(ballPos, boundary.up);
            return (Mathf.Abs(xOffset) <= boundary.localScale.x / 2f) /*&& (Mathf.Abs(zOffset) <= boundary.localScale.z / 2f)*/ && (Mathf.Abs(yOffset) <= boundary.localScale.y / 2f);
        }
    }
}