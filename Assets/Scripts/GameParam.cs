using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	// Hold all parameters for easy adjusting
	[CreateAssetMenu(fileName = "New Game Paremeters", menuName = "Scriptable Object/Game Parameters")]
	public class GameParam : ScriptableObject
	{
		[SerializeField] private float playerHForce;
		[SerializeField] private float playerVForce;
		[SerializeField] private float playerVDiffMultiplier;
		[SerializeField] private float wallHForce;
		[SerializeField] private float wallVForce;
		[SerializeField] private float wallVDiffMultiplier;

		public float PlayerHForce { get => playerHForce; }
		public float PlayerVForce { get => playerVForce; }
		public float PlayerVDiffMultiplier { get => playerVDiffMultiplier; }
		public float WallHForce { get => wallHForce; }
		public float WallVForce { get => wallVForce; }
		public float WallVDiffMultiplier { get => wallVDiffMultiplier; }
	}
}