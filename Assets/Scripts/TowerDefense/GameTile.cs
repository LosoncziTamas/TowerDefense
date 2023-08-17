using UnityEngine;

namespace TowerDefense
{
    public class GameTile : MonoBehaviour
    {
        private static readonly Quaternion
            NorthRotation = Quaternion.Euler(90f, 0f, 0f);

        private static readonly Quaternion
            EastRotation = Quaternion.Euler(90f, 90f, 0f);

        private static readonly Quaternion
            SouthRotation = Quaternion.Euler(90f, 180f, 0f);

        private static readonly Quaternion
            WestRotation = Quaternion.Euler(90f, 270f, 0f);

        [SerializeField] private Transform _arrow;
        private GameTileContent _content;
        private int _distance;

        private GameTile _north, _east, _south, _west, _nextOnPath;

        public GameTileContent Content
        {
            get => _content;
            set
            {
                Debug.Assert(value != null, "Null assigned to content!");
                if (_content != null) _content.Recycle();

                _content = value;
                _content.transform.localPosition = transform.localPosition;
            }
        }

        public bool IsAlternative { get; set; }

        public bool HasPath => _distance != int.MaxValue;

        public void BecomeDestination()
        {
            _distance = 0;
            _nextOnPath = null;
        }

        public void ClearPath()
        {
            _distance = int.MaxValue;
            _nextOnPath = null;
        }

        public GameTile GrowPathNorth()
        {
            return GrowPathTo(_north);
        }

        public GameTile GrowPathEast()
        {
            return GrowPathTo(_east);
        }

        public GameTile GrowPathSouth()
        {
            return GrowPathTo(_south);
        }

        public GameTile GrowPathWest()
        {
            return GrowPathTo(_west);
        }

        private GameTile GrowPathTo(GameTile neighbor)
        {
            Debug.Assert(HasPath, "No path!");
            if (neighbor == null || neighbor.HasPath)
            {
                return null;
            }

            neighbor._distance = _distance + 1;
            neighbor._nextOnPath = this;
            return
                neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
        }

        public void HidePath()
        {
            _arrow.gameObject.SetActive(false);
        }

        public void ShowPath()
        {
            if (_distance == 0)
            {
                _arrow.gameObject.SetActive(false);
                return;
            }

            _arrow.gameObject.SetActive(true);
            _arrow.localRotation =
                _nextOnPath == _north ? NorthRotation :
                _nextOnPath == _east ? EastRotation :
                _nextOnPath == _south ? SouthRotation :
                WestRotation;
        }

        public static void MakeEastWestNeighbors(GameTile east, GameTile west)
        {
            Debug.Assert(
                west._east == null && east._west == null, "Redefined neighbors!"
            );
            west._east = east;
            east._west = west;
        }

        public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
        {
            Debug.Assert(
                south._north == null && north._south == null, "Redefined neighbors!"
            );
            south._north = north;
            north._south = south;
        }
    }
}