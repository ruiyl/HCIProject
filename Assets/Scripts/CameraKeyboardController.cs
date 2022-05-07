using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	// Use only for camera movement in Test scene
	public class CameraKeyboardController : MonoBehaviour
	{
		[SerializeField] private float speed;

		private void Update()
		{
			bool w = Input.GetKey(KeyCode.W);
			bool a = Input.GetKey(KeyCode.A);
			bool s = Input.GetKey(KeyCode.S);
			bool d = Input.GetKey(KeyCode.D);
			bool space = Input.GetKey(KeyCode.Space);
			bool lCtrl = Input.GetKey(KeyCode.LeftControl);

			Vector3 translation = Vector3.zero;
			float zAxis, xAxis, yAxis;
			zAxis = (w ? 1f : 0f) + (s ? -1f : 0f);
			xAxis = (d ? 1f : 0f) + (a ? -1f : 0f);
			yAxis = (space ? 1f : 0f) + (lCtrl ? -1f : 0f);
			translation += transform.forward * (speed * zAxis);
			translation += transform.right * (speed * xAxis);
			translation += transform.up * (speed * yAxis);

			transform.Translate(translation * Time.deltaTime);
		}
	}
}