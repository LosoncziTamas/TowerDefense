using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class GameBoard : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        [SerializeField] private Transform _ground;
        [SerializeField] private GameTile _tilePrefab;
        [SerializeField] private Texture2D _gridTexture;

        private readonly Queue<GameTile> _searchFrontier = new Queue<GameTile>();
        
        private GameTileContentFactory _contentFactory;
        private bool _showGrid, _showPaths;
        private Vector2Int _size;
        private GameTile[] _tiles;

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                var m = _ground.GetComponent<MeshRenderer>().material;
                if (_showGrid)
                {
                    m.mainTexture = _gridTexture;
                    m.SetTextureScale(MainTex, _size);
                }
                else
                {
                    m.mainTexture = null;
                }
            }
        }

        public bool ShowPaths
        {
            get => _showPaths;
            set
            {
                _showPaths = value;
                if (_showPaths)
                    foreach (var tile in _tiles)
                        tile.ShowPath();
                else
                    foreach (var tile in _tiles)
                        tile.HidePath();
            }
        }

        public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
        {
            _size = size;
            _contentFactory = contentFactory;
            _ground.localScale = new Vector3(size.x, size.y, 1f);

            var offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
            _tiles = new GameTile[size.x * size.y];
            for (int i = 0, y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++, i++)
                {
                    var tile = _tiles[i] = Instantiate(_tilePrefab);
                    Transform trans;
                    (trans = tile.transform).SetParent(transform, false);
                    trans.localPosition = new Vector3(
                        x - offset.x, 0f, y - offset.y
                    );

                    if (x > 0) GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                    if (y > 0) GameTile.MakeNorthSouthNeighbors(tile, _tiles[i - size.x]);

                    tile.IsAlternative = (x & 1) == 0;
                    if ((y & 1) == 0) tile.IsAlternative = !tile.IsAlternative;

                    tile.Content = contentFactory.Get(GameTileContentType.Empty);
                }
            }

            ToggleDestination(_tiles[_tiles.Length / 2]);
        }

        public void ToggleDestination(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                if (!FindPaths())
                {
                    tile.Content =
                        _contentFactory.Get(GameTileContentType.Destination);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }

        public void ToggleWall(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Wall)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Wall);
                if (!FindPaths())
                {
                    tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                    FindPaths();
                }
            }
        }

        public GameTile GetTile(Ray ray)
        {
            if (Physics.Raycast(ray, out var hit))
            {
                var x = (int)(hit.point.x + _size.x * 0.5f);
                var y = (int)(hit.point.z + _size.y * 0.5f);
                if (x >= 0 && x < _size.x && y >= 0 && y < _size.y) return _tiles[x + y * _size.x];
            }

            return null;
        }

        private bool FindPaths()
        {
            foreach (var tile in _tiles)
                if (tile.Content.Type == GameTileContentType.Destination)
                {
                    tile.BecomeDestination();
                    _searchFrontier.Enqueue(tile);
                }
                else
                {
                    tile.ClearPath();
                }

            if (_searchFrontier.Count == 0) return false;

            while (_searchFrontier.Count > 0)
            {
                var tile = _searchFrontier.Dequeue();
                if (tile != null)
                {
                    if (tile.IsAlternative)
                    {
                        _searchFrontier.Enqueue(tile.GrowPathNorth());
                        _searchFrontier.Enqueue(tile.GrowPathSouth());
                        _searchFrontier.Enqueue(tile.GrowPathEast());
                        _searchFrontier.Enqueue(tile.GrowPathWest());
                    }
                    else
                    {
                        _searchFrontier.Enqueue(tile.GrowPathWest());
                        _searchFrontier.Enqueue(tile.GrowPathEast());
                        _searchFrontier.Enqueue(tile.GrowPathSouth());
                        _searchFrontier.Enqueue(tile.GrowPathNorth());
                    }
                }
            }

            foreach (var tile in _tiles)
            {
                if (!tile.HasPath)
                {
                    return false;
                }
            }

            if (_showPaths)
            {
                foreach (var tile in _tiles)
                {
                    tile.ShowPath();
                }
            }
            return true;
        }
    }
}