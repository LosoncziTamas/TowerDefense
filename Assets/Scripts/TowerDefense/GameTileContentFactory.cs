using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    [CreateAssetMenu]
    public class GameTileContentFactory : ScriptableObject
    {
        [SerializeField] private GameTileContent _destinationPrefab;
        [SerializeField] private GameTileContent _emptyPrefab;
        [SerializeField] private GameTileContent _wallPrefab;

        private Scene _contentScene;

        public GameTileContent Get(GameTileContentType type)
        {
            switch (type)
            {
                case GameTileContentType.Destination: return Get(_destinationPrefab);
                case GameTileContentType.Empty: return Get(_emptyPrefab);
                case GameTileContentType.Wall: return Get(_wallPrefab);
            }

            Debug.Assert(false, "Unsupported type: " + type);
            return null;
        }

        public void Reclaim(GameTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }

        private GameTileContent Get(GameTileContent prefab)
        {
            var instance = Instantiate(prefab);
            instance.OriginFactory = this;
            MoveToFactoryScene(instance.gameObject);
            return instance;
        }

        private void MoveToFactoryScene(GameObject o)
        {
            if (!_contentScene.isLoaded)
            {
                if (Application.isEditor)
                {
                    _contentScene = SceneManager.GetSceneByName(name);
                    if (!_contentScene.isLoaded) _contentScene = SceneManager.CreateScene(name);
                }
                else
                {
                    _contentScene = SceneManager.CreateScene(name);
                }
            }

            SceneManager.MoveGameObjectToScene(o, _contentScene);
        }
    }
}