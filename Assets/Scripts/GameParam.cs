using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "New Game Paremeters", menuName = "Scriptable Object/Game Parameters")]
	public class GameParam : ScriptableObject
	{
		[SerializeField] private float playerForce;
		[SerializeField] private float wallForce;

		public float PlayerForce { get => playerForce; }
		public float WallForce { get => wallForce; }
	}
}