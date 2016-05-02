using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class Board
    {
        struct RowShiftInfo
        {
            public ushort row;
            public int reward;
        }
        struct ColumnShiftInfo
        {
            public ulong column;
            public int reward;
        }
        static RowShiftInfo[] RowShiftLeftTable = new RowShiftInfo[65536];
        static RowShiftInfo[] RowShiftRightTable = new RowShiftInfo[65536];
        static ColumnShiftInfo[] ColumnShiftUpTable = new ColumnShiftInfo[65536];
        static ColumnShiftInfo[] ColumnShiftDownTable = new ColumnShiftInfo[65536];

        public ulong blocks;
        public int score;
        public int EmptyCount
        {
            get
            {
                int result = 0;
                for (int i = 0; i < 64; i += 4)
                {
                    if (((blocks >> i) & 0xf) == 0)
                    {
                        result++;
                    }
                }
                return result;
            }
        }
        public bool CanMove
        {
            get
            {
                int reward;
                for (int i = (int)Direction.Up; i < (int)Direction.Right; i++)
                {
                    if (blocks != Move((Direction)i, out reward))
                        return true;
                }
                return false;
            }
        }

        static Board()
        {
            for (int row = 0; row < 65536; row++)
            {
                int reward = 0;
                int[] lines = 
                    {
                       (row >>  0) & 0xf,
                       (row >>  4) & 0xf,
                       (row >>  8) & 0xf,
                       (row >> 12) & 0xf
                    };
                // execute a move to the left
                for (int i = 0; i < 3; ++i)
                {
                    int j;
                    for (j = i + 1; j < 4; ++j)
                    {
                        if (lines[j] != 0) break;
                    }
                    if (j == 4) break; // no more tiles to the right

                    if (lines[i] == 0)
                    {
                        lines[i] = lines[j];
                        lines[j] = 0;
                        i--; // retry this entry
                    }
                    else if (lines[i] == lines[j])
                    {
                        if (lines[i] != 0xf)
                        {
                            /* Pretend that 32768 + 32768 = 32768 (representational limit). */
                            lines[i]++;
                        }
                        lines[j] = 0;
                        reward += 2 << (lines[i]-1);
                    }
                }

                int result = (lines[0] << 0) |
                             (lines[1] << 4) |
                             (lines[2] << 8) |
                             (lines[3] << 12);
                int rev_result = ReverseRow(result);
                int rev_row = ReverseRow(row);

                RowShiftLeftTable[row] =new RowShiftInfo { row = (ushort)(row ^ result), reward = reward };
                RowShiftRightTable[rev_row] = new RowShiftInfo { row = (ushort)(rev_row ^ rev_result), reward = reward };
                ColumnShiftUpTable[row] = new ColumnShiftInfo { column = UnpackColumn(row) ^ UnpackColumn(result), reward = reward };
                ColumnShiftDownTable[rev_row] = new ColumnShiftInfo { column = UnpackColumn(rev_row) ^ UnpackColumn(rev_result), reward = reward };
            }
        }
        static public ushort ReverseRow(int row)
        {
            return (ushort)((row >> 12) | ((row >> 4) & 0x00F0) | ((row << 4) & 0x0F00) | (row << 12));
        }
        static public ulong UnpackColumn(int column)
        {
            ulong tmp = (ulong)column;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) & 0x000F000F000F000F;
        }
        static public ulong Transpose(ulong board)
        {
            ulong result;
            ulong diagonal4x4block = board & 0xFF00FF0000FF00FF;
            ulong topRight4x4block = board & 0x00FF00FF00000000; ;
            ulong downLeft4x4block = board & 0x00000000FF00FF00;

            ulong swaped = diagonal4x4block | (topRight4x4block >> 24) | (downLeft4x4block << 24);

            ulong diagonalNet = swaped & 0xF0F00F0FF0F00F0F;
            ulong upperSparse4corner = swaped & 0x0F0F00000F0F0000; ;
            ulong lowerSparse4corner = swaped & 0x0000F0F00000F0F0;

            result = diagonalNet | (upperSparse4corner >> 12) | (lowerSparse4corner << 12);
            return result;
        }

        public void Print()
        {
            Console.WriteLine(score);
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    Console.Write("\t{0}", (blocks>>(16*i+4*j))&0xf);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void InsertNewTile()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            ulong tile = (random.Next(0, 9) == 0) ? 2UL : 1UL;
            int emptyCount = EmptyCount;
            if (emptyCount > 0)
            {
                int targetIndex = random.Next(0, emptyCount - 1);
                for(int i = 0; i < 64; i += 4)
                {
                    if(((blocks>>i)&0xf) == 0)
                    {
                        if (targetIndex == 0)
                        {
                            blocks |= tile << i;
                            break;
                        }
                        else
                        {
                            targetIndex--;
                        }
                    }
                }
            }
        }
        public void Initial()
        {
            InsertNewTile();
            InsertNewTile();
        }
        public ulong Move(Direction direction, out int reward)
        {
            ulong ret, t;
            reward = 0;
            switch (direction)
            {
                case Direction.Up:
                    ret = blocks;
                    t = Transpose(blocks);
                    ret ^= ColumnShiftUpTable[(t >> 0) & 0xFFFF].column << 0;
                    ret ^= ColumnShiftUpTable[(t >> 16) & 0xFFFF].column << 4;
                    ret ^= ColumnShiftUpTable[(t >> 32) & 0xFFFF].column << 8;
                    ret ^= ColumnShiftUpTable[(t >> 48) & 0xFFFF].column << 12;
                    reward += ColumnShiftUpTable[(t >> 0) & 0xFFFF].reward;
                    reward += ColumnShiftUpTable[(t >> 16) & 0xFFFF].reward;
                    reward += ColumnShiftUpTable[(t >> 32) & 0xFFFF].reward;
                    reward += ColumnShiftUpTable[(t >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Down:
                    ret = blocks;
                    t = Transpose(blocks);
                    ret ^= ColumnShiftDownTable[(t >> 0) & 0xFFFF].column << 0;
                    ret ^= ColumnShiftDownTable[(t >> 16) & 0xFFFF].column << 4;
                    ret ^= ColumnShiftDownTable[(t >> 32) & 0xFFFF].column << 8;
                    ret ^= ColumnShiftDownTable[(t >> 48) & 0xFFFF].column << 12;
                    reward += ColumnShiftDownTable[(t >> 0) & 0xFFFF].reward;
                    reward += ColumnShiftDownTable[(t >> 16) & 0xFFFF].reward;
                    reward += ColumnShiftDownTable[(t >> 32) & 0xFFFF].reward;
                    reward += ColumnShiftDownTable[(t >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Left:
                    ret = blocks;
                    ret ^= ((ulong)(RowShiftLeftTable[(blocks >> 0) & 0xFFFF].row)) << 0;
                    ret ^= ((ulong)(RowShiftLeftTable[(blocks >> 16) & 0xFFFF].row)) << 16;
                    ret ^= ((ulong)(RowShiftLeftTable[(blocks >> 32) & 0xFFFF].row)) << 32;
                    ret ^= ((ulong)(RowShiftLeftTable[(blocks >> 48) & 0xFFFF].row)) << 48;
                    reward += RowShiftLeftTable[(blocks >> 0) & 0xFFFF].reward;
                    reward += RowShiftLeftTable[(blocks >> 16) & 0xFFFF].reward;
                    reward += RowShiftLeftTable[(blocks >> 32) & 0xFFFF].reward;
                    reward += RowShiftLeftTable[(blocks >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Right:
                    ret = blocks;
                    ret ^= ((ulong)(RowShiftRightTable[(blocks >> 0) & 0xFFFF].row)) << 0;
                    ret ^= ((ulong)(RowShiftRightTable[(blocks >> 16) & 0xFFFF].row)) << 16;
                    ret ^= ((ulong)(RowShiftRightTable[(blocks >> 32) & 0xFFFF].row)) << 32;
                    ret ^= ((ulong)(RowShiftRightTable[(blocks >> 48) & 0xFFFF].row)) << 48;
                    reward += RowShiftRightTable[(blocks >> 0) & 0xFFFF].reward;
                    reward += RowShiftRightTable[(blocks >> 16) & 0xFFFF].reward;
                    reward += RowShiftRightTable[(blocks >> 32) & 0xFFFF].reward;
                    reward += RowShiftRightTable[(blocks >> 48) & 0xFFFF].reward;
                    return ret;
                default:
                    return blocks;
            }
        }
    }
}
