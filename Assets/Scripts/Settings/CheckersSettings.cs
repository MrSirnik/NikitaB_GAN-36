using UnityEngine;

namespace Tactics
{
    [CreateAssetMenu(menuName = "Tactics/Checkers Settings", fileName = "CheckersSettings")]
    public class CheckersSettings : ScriptableObject
    {
        public Color player1Color = new(0.9f, 0.9f, 0.92f);
        public Color player2Color = new(0.12f, 0.12f, 0.14f);

        public float manHeight = 0.25f;
        public float manRadius = 0.35f;
        public float kingHeight = 0.4f;
        public float kingCrownScale = 0.5f;

        public float moveDuration = 0.35f;
        public float captureRemoveDelay = 0.15f;
    }
}
