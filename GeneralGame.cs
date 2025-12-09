using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint_3
{
    public class GeneralGame : BaseGame
    {
        public GeneralGame(int boardSize) : base(boardSize) { }

        public override void MakeMove(int row, int col, Cell move)
        {
            if (State != GameState.InProgress || GameBoard[row, col] != Cell.Empty)
            {
                return;
            }

            Player playerMoving = CurrentTurn;

            GameBoard[row, col] = move;

            RecordMove(row, col, move, playerMoving);

            MovesLeft--;
            CheckForWin(row, col);
        }

        public override int TestMove(int row, int col, Cell move)
        {
            if (GameBoard[row, col] != Cell.Empty)
            {
                return 0;
            }

            GameBoard[row, col] = move;

            int sosCount = FindSOS(row, col);

            GameBoard[row, col] = Cell.Empty;

            return sosCount;
        }
        protected override void CheckForWin(int r, int c)
        {
            int newSOSCount = FindSOS(r, c);

            if (newSOSCount > 0)
            {
                if (CurrentTurn == Player.Blue)
                {
                    BlueScore += newSOSCount;
                }
                else
                {
                    RedScore += newSOSCount;
                }
            }
            else
            {
                CurrentTurn = (CurrentTurn == Player.Blue) ? Player.Red : Player.Blue;
            }

            if (MovesLeft == 0)
            {
                if (BlueScore > RedScore)
                {
                    State = GameState.BlueWin;
                }
                else if(RedScore > BlueScore) {
                    State = GameState.RedWin;
                }
                else
                {
                    State = GameState.Draw;
                }
            }
        }
        private int FindSOS(int r, int c)
        {
            int sosCount = 0;
            Cell move = GameBoard[r, c];

            int[,] directions =
            {
                { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 },
                { 0, -1 }, { -1, 0 }, { -1, -1 }, { -1, -1 }
            };

            if (move == Cell.O)
            {
                for (int i = 0; i < 4; i++)
                {
                    int r1 = r + directions[i, 0];
                    int c1 = c + directions[i, 1];
                    int r2 = r - directions[i, 0];
                    int c2 = c - directions[i, 1];

                    if (IsOnBoard(r1, c1) && GameBoard[r1, c1] == Cell.S && IsOnBoard(r2, c2) && GameBoard[r2, c2] == Cell.S)
                    {
                        sosCount++;
                    }
                }
            }
            else if (move == Cell.S)
            {
                for (int i = 0; i < 8; i++)
                {
                    int r1 = r + directions[i, 0];
                    int c1 = c + directions[i, 1];
                    int r2 = r + 2 * directions[i, 0];
                    int c2 = c + 2 * directions[i, 1];

                    if (IsOnBoard(r1, c1) && GameBoard[r1, c1] == Cell.O && IsOnBoard(r2, c2) && GameBoard[r2, c2] == Cell.S)
                    {
                        sosCount++;
                    }
                }
            }
            return sosCount;
        }

        private bool IsOnBoard(int r, int c)
        {
            return r >= 0 && r < BoardSize && c >= 0 && c < BoardSize;
        }
    }
}
