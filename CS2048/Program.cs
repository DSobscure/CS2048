using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            board.Initial();
            board.Print();
            while(board.CanMove)
            {
                int reward = 0;
                switch (Console.ReadLine())
                {
                    case "w":
                        board.blocks = board.Move(Direction.Up, out reward);
                        break;
                    case "s":
                        board.blocks = board.Move(Direction.Down, out reward);
                        break;
                    case "a":
                        board.blocks = board.Move(Direction.Left, out reward);
                        break;
                    case "d":
                        board.blocks = board.Move(Direction.Right, out reward);
                        break;
                }
                board.score += reward;
                board.InsertNewTile();
                board.Print();
            }
            Console.Read();
        }
    }
}
