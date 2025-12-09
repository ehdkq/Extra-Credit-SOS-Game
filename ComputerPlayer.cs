using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sprint_3
{
    public enum Difficulty { Easy, Hard}
    public class ComputerPlayer : IPlayer
    {
        private Random _rand = new Random();

        public Difficulty CurrentDifficulty { get; set; }

        public ComputerPlayer(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
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

            var emptySpots = new List<Point>();

            for (int r = 0;r < game.BoardSize; r++)
            {
                for (int c = 0;c < game.BoardSize; c++)
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

            Point target = emptySpots[_rand.Next(emptySpots.Count)];

            Cell randomLetter = (_rand.Next(0, 2) == 0) ? Cell.S : Cell.O;

            game.MakeMove(target.X, target.Y, randomLetter);

        }
    }
}