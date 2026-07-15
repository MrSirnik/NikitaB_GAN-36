using UnityEngine;

namespace Tactics
{
    // Метка на сцене: все объекты уже размещены заранее в редакторе,
    // TacticsBootstrap.Run() не должен пересобирать их с нуля при каждом запуске Play.
    public class BakedTacticsMarker : MonoBehaviour
    {
    }
}
