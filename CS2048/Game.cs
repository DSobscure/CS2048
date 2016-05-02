using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class Game
    {
        public Board Board { get; private set; }
        public int Score { get; private set; }
        public bool IsEnd
        {
            get
            {
                return false;
                //bool result = false;
                //for(int i = (int)Direction.Up; i < (int)Direction.Right; i++)
                //{
                //    if()
                //}
            }
        }
        public bool MoveCheck(Direction direction)
        {
            return false;
        }
        public bool Move(Direction direction, out double reward)
        {
            reward = 0;
            return false;
        }
    }
}
