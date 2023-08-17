using UnityEngine;

namespace TowerDefense
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Vector2Int _boardSize = new Vector2Int(11, 11);
        [SerializeField] private GameBoard _board;
        [SerializeField] private GameTileContentFactory _tileContentFactory;

        private static Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        private void Awake()
        {
            _board.Initialize(_boardSize, _tileContentFactory);
            _board.ShowGrid = true;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleAlternativeTouch();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                _board.ShowPaths = !_board.ShowPaths;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                _board.ShowGrid = !_board.ShowGrid;
            }
        }

        private void OnValidate()
        {
            if (_boardSize.x < 2)
            {
                _boardSize.x = 2;
            }
            if (_boardSize.y < 2)
            {
                _boardSize.y = 2;
            }
        }

        private void HandleAlternativeTouch()
        {
            var tile = _board.GetTile(TouchRay);
            if (tile != null)
            {
                _board.ToggleDestination(tile);
            }
        }

        private void HandleTouch()
        {
            var tile = _board.GetTile(TouchRay);
            if (tile != null)
            {
                _board.ToggleWall(tile);
            }
        }
    }
}