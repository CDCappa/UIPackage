using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace UIP.Runtime.Core.SceneManagement
{
    public class UIPScenePurposeInternal : ScriptableObject
    {
        [SerializeField] [HideInInspector] protected List<Scene> scenes = new List<Scene>();

        public Scene GetScene(string name)
        {
            return scenes.FirstOrDefault((scene) => scene.Asset.name.Equals(name));
        }

        public Scene GetScene(ScenePurpose purpose)
        {
            return scenes.FirstOrDefault((scene) => scene.Purpose.Equals(purpose));
        }

        public void SetSceneAsset(ScenePurpose purpose, SceneAsset sceneAsset)
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].Purpose.Equals(purpose))
                {
                    scenes[i] = new Scene(purpose, sceneAsset);
                    return;
                }
            }

            scenes.Add(new Scene(purpose, sceneAsset));
        }
    }
}