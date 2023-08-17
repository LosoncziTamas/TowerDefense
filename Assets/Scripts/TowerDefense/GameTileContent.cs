using UnityEngine;

namespace TowerDefense
{
    public class GameTileContent : MonoBehaviour
    {
        [SerializeField] private GameTileContentType _type;

        private GameTileContentFactory _originFactory;

        public GameTileContentType Type => _type;

        public GameTileContentFactory OriginFactory
        {
            get => _originFactory;
            set
            {
                Debug.Assert(_originFactory == null, "Redefined origin factory!");
                _originFactory = value;
            }
        }

        public void Recycle()
        {
            _originFactory.Reclaim(this);
        }
    }
}