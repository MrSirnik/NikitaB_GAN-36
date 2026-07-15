using UnityEngine;

namespace Tactics
{
    [CreateAssetMenu(menuName = "Tactics/Board Settings", fileName = "BoardSettings")]
    public class BoardSettings : ScriptableObject
    {
        [Min(2)] public int size = 8;
        public float cellSize = 1f;

        public Color lightCellColor = new(0.85f, 0.8f, 0.7f);
        public Color darkCellColor = new(0.32f, 0.22f, 0.15f);
        public Color hoverColor = new(0.9f, 0.9f, 0.4f);
        public Color selectColor = new(0.3f, 0.85f, 0.3f);
        public Color moveColor = new(0.35f, 0.65f, 0.95f);
        public Color pendingColor = new(1f, 0.75f, 0.1f);
    }
}
