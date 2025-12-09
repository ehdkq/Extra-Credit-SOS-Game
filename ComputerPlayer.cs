using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sprint_3
{
    public enum Difficulty { Easy, Hard }

    public class ComputerPlayer : IPlayer
    {
        private Random _rand = new Random();

        public Difficulty CurrentDifficulty { get; set; }

        public ComputerPlayer(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
        }

        private bool IsMoveSafe(BaseGame game, int row, int col, Cell letter)
        {
            game.GameBoard[row, col] = letter;

            bool isSafe = true;

            for (int r = 0; r < game.BoardSize; r++)
            {
                for (int c = 0; c < game.BoardSize; c++)
                {
                    if (game.GameBoard[r, c] == Cell.Empty)
                    {
                        if (game.TestMove(r, c, Cell.S) > 0 || game.TestMove(r, c, Cell.O) > 0)
                        {
                            isSafe = false;
                            break;
                        }
                    }
                }
                if (!isSafe)
                {
                    break;
                }
            }
            game.GameBoard[row, col] = Cell.Empty;

            return isSafe;
        }

        public void MakeMove(BaseGame game)
        {
            if (CurrentDifficulty == Difficulty.Hard)
            {
                for (int r = 0; r < game.BoardSize; r++)
                {
                    for (int c = 0; c < game.BoardSize; c++)
                    {
                        if (game.GameBoard[r, c] == Cell.Empty)
                        {
                            if (game.TestMove(r, c, Cell.S) > 0)
                            {
                                game.MakeMove(r, c, Cell.S);
                                return;
                            }
                            if (game.TestMove(r, c, Cell.O) > 0)
                            {
                                game.MakeMove(r, c, Cell.O);
                                return;
                            }
                        }
                    }
                }
            }

            var emptySpots = new List<Point>();
            for (int r = 0; r < game.BoardSize; r++)
            {
                for (int c = 0; c < game.BoardSize; c++)
                {
                    if (game.GameBoard[r, c] == Cell.Empty)
                    {
                        emptySpots.Add(new Point(r, c));
                    }
                }
            }

            if (emptySpots.Count == 0)
            {
                return;
            }

            if (CurrentDifficulty == Difficulty.Hard)
            {
                List<Point> shuffledSpots = emptySpots.OrderBy(x => _rand.Next()).ToList();

                foreach (Point p in shuffledSpots)
                {
                    if (IsMoveSafe(game, p.X, p.Y, Cell.S))
                    {
                        game.MakeMove(p.X, p.Y, Cell.S);
                        return;
                    }
                    if (IsMoveSafe(game, p.X, p.Y, Cell.O))
                    {
                        game.MakeMove(p.X, p.Y, Cell.O);
                        return;
                    }
                }
            }

            Point target = emptySpots[_rand.Next(emptySpots.Count)];
            Cell randomLetter = (_rand.Next(0, 2) == 0) ? Cell.S : Cell.O;

            game.MakeMove(target.X, target.Y, randomLetter);
        }
    }
}