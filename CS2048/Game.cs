using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class Game
    {
        public Board board;
        public int score;
        public bool IsEnd
        {
            get
            {
                return !board.CanMove;
            }
        }
        public Game()
        {
            board = new Board();
            board.Initial();
        }
    }
}
