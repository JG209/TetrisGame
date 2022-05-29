using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris
{
    public class Board : MonoBehaviour
    {
        public Tilemap tilemap {get; private set;}
        public Piece activePiece {get; private set;}
        public TetrominoData[] tetrominos;
        public Vector3Int spanwPosition;
        public Vector2Int boardSize = new Vector2Int(10, 20);

        public RectInt Bounds
        {
            get
            {
                Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
                return new RectInt(position, boardSize);
            }
        }
        
        void Awake()
        {
            tilemap = GetComponentInChildren<Tilemap>();
            activePiece = GetComponentInChildren<Piece>();

            for (int i = 0; i < tetrominos.Length; i++)
            {
                tetrominos[i].Initialize();
            }
            
        }
        
        void Start()
        {
            SpawnPiece();
        }

        

        public void SpawnPiece()
        {
            int randomIndex = Random.Range(0, tetrominos.Length);
            TetrominoData data = tetrominos[randomIndex];

            activePiece.Initialize(this, spanwPosition, data);

            if(IsValidPosition(activePiece, spanwPosition))
                SetPiece(activePiece);
            else
                GameOver();

        }

        public void SetPiece(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, piece.data.tile);
            }
        }

        public void ClearPiece(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, null);
            }
        }

        public bool IsValidPosition(Piece piece, Vector3Int position)//Check if the next piece position is valid
        {
            RectInt bounds = Bounds;

            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + position;

                if(!bounds.Contains((Vector2Int)tilePosition))
                {
                    return false;
                }

                if(tilemap.HasTile(tilePosition))
                {
                    return false;
                }

            }
            
            return true;    
        }

        public void ClearLines()
        {
            RectInt bounds = Bounds;
            int row = bounds.yMin;

            while (row < bounds.yMax)
            {
                if(isLineFull(row))
                    LineClear(row);
                else
                    row++;
            }
        }

        private bool isLineFull(int row)
        {
            RectInt bounds = Bounds;

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if(!tilemap.HasTile(position))
                {
                    return false;
                }
            }

            return true;
        }

        private void LineClear(int row)
        {
            RectInt bounds = Bounds;

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, null);
            }

            while(row < bounds.yMax)
            {
                for (int col = bounds.xMin; col < bounds.xMax; col++)
                {
                    Vector3Int position = new Vector3Int(col, row + 1, 0);
                    TileBase above = tilemap.GetTile(position);

                    position = new Vector3Int(col, row, 0);
                    tilemap.SetTile(position, above);
                }

                row++;
            }
        }

        private void GameOver()
        {
            tilemap.ClearAllTiles();

            //....
        }
    }
}
