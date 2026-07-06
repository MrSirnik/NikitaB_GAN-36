namespace FPS
{
    public enum SurfaceType
    {
        Stone,
        Metal,
        Wood,
        Sand,
        Flesh
    }

    public class SurfaceMarker : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private SurfaceType _surface = SurfaceType.Stone;

        public SurfaceType Surface => _surface;
        public void SetSurface(SurfaceType surface) => _surface = surface;
    }
}
