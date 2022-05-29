using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class Piece : MonoBehaviour
    {
        [System.Serializable]
        public class Inputs
        {
            public bool moveLeft;
            public bool moveRight;
            public bool rotateLeft;
            public bool rotateRight;
            public bool moveDown;
            public bool hardDrop;

            public void UpdateInputs()
            {
                moveLeft = Input.GetKeyDown(KeyCode.A);
                moveRight = Input.GetKeyDown(KeyCode.D);
                rotateLeft = Input.GetKeyDown(KeyCode.Q);
                rotateRight = Input.GetKeyDown(KeyCode.E);
                moveDown = Input.GetKeyDown(KeyCode.S);
                hardDrop = Input.GetKeyDown(KeyCode.Space);
            }
        }
        Inputs inputs = new Inputs();

        public Board board {get; private set;}
        public TetrominoData data {get; private set;}
        public Vector3Int[] cells {get; private set;}
        public Vector3Int position {get; private set;}
        public int rotationIndex {get; private set;}

        public float stepDelay = 1f;
        public float lockDelay = .5f;

        private float stepTime;
        private float lockTime;



        public void Initialize(Board board, Vector3Int position, TetrominoData data)
        {
            this.board = board;
            this.position = position;
            this.data = data;
            rotationIndex = 0;
            stepTime = Time.time + stepDelay;
            lockTime = 0f;

            if(cells == null)
                cells = new Vector3Int[data.cells.Length];

            for (int i = 0; i < data.cells.Length; i++)
            {
                cells[i] = (Vector3Int)data.cells[i];
            }
        }

        private void Update()
        {
            inputs.UpdateInputs();

            board.ClearPiece(this);

            lockTime += Time.deltaTime;

            if(inputs.rotateLeft)
                RotatePiece(-1);
            else if(inputs.rotateRight)
                RotatePiece(1);

            if(inputs.moveLeft)
                MovePiece(Vector2Int.left);
            else if(inputs.moveRight)
                MovePiece(Vector2Int.right);

            if(inputs.moveDown)
                MovePiece(Vector2Int.down);

            if(inputs.hardDrop)
                HardDrop();
            
            if(Time.time >= stepTime)
                Step();

            board.SetPiece(this);
        }

        private void Step()
        {
            stepTime = Time.time + stepDelay;

            MovePiece(Vector2Int.down);

            if(lockTime >= lockDelay)
                Lock();
        }

        private void Lock()
        {
            board.SetPiece(this);
            board.ClearLines();
            board.SpawnPiece();
        }

        private void HardDrop()
        {
            while(MovePiece(Vector2Int.down))
            {
                continue;
            }
        }

        private bool MovePiece(Vector2Int translation)
        {
            Vector3Int newPosition = position;
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            bool valid = board.IsValidPosition(this, newPosition);

            if(valid)
            {
                position = newPosition;
                lockTime = 0f;
            }

            return valid;
        }

        private void RotatePiece(int direction)
        {
            int originalRotation = rotationIndex;
            rotationIndex = Wrap(rotationIndex + direction, 0, 4);

            ApplyRotationMatrix(direction);

            if(!TestWallKick(rotationIndex, direction))
            {
                rotationIndex = originalRotation;
                ApplyRotationMatrix(-direction);
            }
        }

        private void ApplyRotationMatrix(int direction)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                int x, y;

                switch(data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.O:
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                        y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                    default:
                        x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                        y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                }

                cells[i] = new Vector3Int(x, y, 0);
            }
        }

        private bool TestWallKick(int indexRotation, int rotationDirection)
        {
            int wallKickIndex = GetWallKickIndex(indexRotation, rotationDirection);

            for (int i = 0; i < data.wallKicks.GetLength(1); i++)
            {
                Vector2Int translation = data.wallKicks[wallKickIndex, i];

                if(MovePiece(translation))
                    return true;
            }

            return false;
        }

        private int GetWallKickIndex(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = rotationIndex * 2;

            if(rotationDirection < 0)
                wallKickIndex--;

            return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
        }

        private int Wrap(int input, int min, int max)
        {
            if(input < min)
                return max - (min - input) % (max - min);
            else
                return min + (input - min) % (max - min);
        }
    }
}
