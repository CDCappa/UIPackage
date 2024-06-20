using System;

using UnityEditor;

namespace UIP.Runtime.Core.SceneManagement
{
    [Serializable]
    public struct Scene
    {
        public ScenePurpose Purpose;
        public SceneAsset Asset;

        public Scene(ScenePurpose purpose, SceneAsset asset)
        {
            Purpose = purpose;
            Asset = asset;
        }
    }
}