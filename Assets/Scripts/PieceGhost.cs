using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris
{
    public class PieceGhost : MonoBehaviour
    {
        public Tile tile;
        public Board board;
        public Piece trackingPiece;

        public Tilemap tilemap {get; private set;}
        public Vector3Int[] cells {get; private set;}
        public Vector3Int position {get; private set;}

        private void Awake()
        {
            tilemap = GetComponentInChildren<Tilemap>();
            cells = new Vector3Int[4];
        }

        void LateUpdate()
        {
            Clear();
            Copy();
            Drop();
            Set();
        }

        private void Clear()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int tilePosition = cells[i] + position;
                tilemap.SetTile(tilePosition, null);
            }
        }
        private void Copy()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = trackingPiece.cells[i];
            }
        }
        private void Drop()
        {
            Vector3Int dropPosition = trackingPiece.position;

            int currentRow = dropPosition.y;
            int boardBottom = -board.boardSize.y / 2 - 1;

            board.ClearPiece(trackingPiece);

            for (int row = currentRow; row >= boardBottom; row--)
            {
                dropPosition.y = row;

                if(board.IsValidPosition(trackingPiece, dropPosition))
                    position = dropPosition;
                else
                    break;
            }

            board.SetPiece(trackingPiece);
        }
        private void Set()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int tilePosition = cells[i] + position;
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }
}
