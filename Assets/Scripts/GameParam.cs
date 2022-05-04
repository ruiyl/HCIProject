using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "New Game Paremeters", menuName = "Scriptable Object/Game Parameters")]
    public class GameParam : ScriptableObject
    {
        [SerializeField] private float force;

        public float Force { get => force; }
    }
}