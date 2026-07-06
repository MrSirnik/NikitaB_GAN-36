using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FPS
{
    public static class ProjectAsset
    {
        public static T Load<T>(string assetPath) where T : Object
        {
#if UNITY_EDITOR
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                Debug.LogWarning($"ProjectAsset: не найден ассет по пути {assetPath}");
            }
            return asset;
#else
            Debug.LogWarning($"ProjectAsset.Load работает только в редакторе: {assetPath}");
            return null;
#endif
        }
    }
}
