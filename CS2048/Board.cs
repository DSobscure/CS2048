using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class Board
    {
        public ulong blocks;
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
                for (int i = (int)Direction.Up; i <= (int)Direction.Right; i++)
                {
                    if (MoveCheck((Direction)i))
                        return true;
                }
                return false;
            }
        }
        public bool IsFull
        {
            get
            {
                for (int i = 0; i < 64; i += 4)
                {
                    if (((blocks >> i) & 0xf) == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public int MaxTile
        {
            get
            {
                int maxTile = 0;
                for (int i = 0; i < 64; i += 4)
                {
                    int val = (int)((blocks >> i) & 0xf);
                    if (Tables.TileScoreTable[val] > maxTile)
                    {
                        maxTile = Tables.TileScoreTable[val];
                    }
                }
                return maxTile;
            }
        }
        public int DistinctTileCount
        {
            get
            {
                HashSet<int> tileSet = new HashSet<int>();
                for (int i = 0; i < 64; i += 4)
                {
                    int val = (int)((blocks >> i) & 0xf);
                    if (val != 0 && !tileSet.Contains(val))
                    {
                        tileSet.Add(val);
                    }
                }
                return tileSet.Count;
            }
        }
        public int RepeatedTileCount
        {
            get
            {
                HashSet<int> tileSet = new HashSet<int>();
                int count = 0;
                for (int i = 0; i < 64; i += 4)
                {
                    int val = (int)((blocks >> i) & 0xf);
                    if (val != 0)
                    {
                        if(tileSet.Contains(val))
                        {
                            count++;
                        }
                        else
                            tileSet.Add(val);
                    }
                }
                return count;
            }
        }

        public static ushort ReverseRow(int row)
        {
            return (ushort)((row >> 12) | ((row >> 4) & 0x00F0) | ((row << 4) & 0x0F00) | (row << 12));
        }
        public static ulong UnpackColumn(int column)
        {
            ulong tmp = (ulong)column;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) & 0x000F000F000F000F;
        }
        public static ulong Transpose(ulong board)
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
        public static float GetScore(ulong board)
        {
            float score = 0;
            for (int i = 0; i < 4; i++)
            {
                score += Tables.LineScoreTable[GetRow(board, 3 - i),i];
            }
            return score;
        }
        public static ushort GetRow(ulong board, int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return (ushort)((board >> 48) & 0xFFFF);
                case 1:
                    return (ushort)((board >> 32) & 0xFFFF);
                case 2:
                    return (ushort)((board >> 16) & 0xFFFF);
                case 3:
                    return (ushort)((board) & 0xFFFF);
            }
            return (ushort)((board) & 0xFFFF);
        }
        public static ushort GetColumn(ulong board, int columnIndex)
        {
            ulong columnMask = (0xf000f000f000f000 >> (columnIndex * 4));

            board = (board & columnMask) << 4 * columnIndex;
            return (ushort)(GetRow(board, 0) | (GetRow(board, 1) >> 4) | (GetRow(board, 2) >> 8) | (GetRow(board, 3) >> 12));
        }
        public static ulong SetRows(ushort[] rows)
        {
            ulong result = 0;
            result |= (ulong)rows[0] << 48;
            result |= (ulong)rows[1] << 32;
            result |= (ulong)rows[2] << 16;
            result |= rows[3];
            return result;
        }
        public static ulong SetColumns(ushort[] columns)
        {
            return Transpose(SetRows(columns));
        }

        public Board()
        {

        }
        public Board(ulong blocks)
        {
            this.blocks = blocks;
        }
        public void Print()
        {
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    if(((blocks >> (16 * i + 4 * j)) & 0xf) > 0)
                        Console.Write("\t{0}", 1 << (int)((blocks>>(16*i+4*j))&0xf));
                    else
                        Console.Write("\t0");
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
                    ret ^= Tables.ColumnShiftUpTable[(t >> 0) & 0xFFFF].column << 0;
                    ret ^= Tables.ColumnShiftUpTable[(t >> 16) & 0xFFFF].column << 4;
                    ret ^= Tables.ColumnShiftUpTable[(t >> 32) & 0xFFFF].column << 8;
                    ret ^= Tables.ColumnShiftUpTable[(t >> 48) & 0xFFFF].column << 12;
                    reward += Tables.ColumnShiftUpTable[(t >> 0) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftUpTable[(t >> 16) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftUpTable[(t >> 32) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftUpTable[(t >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Down:
                    ret = blocks;
                    t = Transpose(blocks);
                    ret ^= Tables.ColumnShiftDownTable[(t >> 0) & 0xFFFF].column << 0;
                    ret ^= Tables.ColumnShiftDownTable[(t >> 16) & 0xFFFF].column << 4;
                    ret ^= Tables.ColumnShiftDownTable[(t >> 32) & 0xFFFF].column << 8;
                    ret ^= Tables.ColumnShiftDownTable[(t >> 48) & 0xFFFF].column << 12;
                    reward += Tables.ColumnShiftDownTable[(t >> 0) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftDownTable[(t >> 16) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftDownTable[(t >> 32) & 0xFFFF].reward;
                    reward += Tables.ColumnShiftDownTable[(t >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Left:
                    ret = blocks;
                    ret ^= ((ulong)(Tables.RowShiftLeftTable[(blocks >> 0) & 0xFFFF].row)) << 0;
                    ret ^= ((ulong)(Tables.RowShiftLeftTable[(blocks >> 16) & 0xFFFF].row)) << 16;
                    ret ^= ((ulong)(Tables.RowShiftLeftTable[(blocks >> 32) & 0xFFFF].row)) << 32;
                    ret ^= ((ulong)(Tables.RowShiftLeftTable[(blocks >> 48) & 0xFFFF].row)) << 48;
                    reward += Tables.RowShiftLeftTable[(blocks >> 0) & 0xFFFF].reward;
                    reward += Tables.RowShiftLeftTable[(blocks >> 16) & 0xFFFF].reward;
                    reward += Tables.RowShiftLeftTable[(blocks >> 32) & 0xFFFF].reward;
                    reward += Tables.RowShiftLeftTable[(blocks >> 48) & 0xFFFF].reward;
                    return ret;
                case Direction.Right:
                    ret = blocks;
                    ret ^= ((ulong)(Tables.RowShiftRightTable[(blocks >> 0) & 0xFFFF].row)) << 0;
                    ret ^= ((ulong)(Tables.RowShiftRightTable[(blocks >> 16) & 0xFFFF].row)) << 16;
                    ret ^= ((ulong)(Tables.RowShiftRightTable[(blocks >> 32) & 0xFFFF].row)) << 32;
                    ret ^= ((ulong)(Tables.RowShiftRightTable[(blocks >> 48) & 0xFFFF].row)) << 48;
                    reward += Tables.RowShiftRightTable[(blocks >> 0) & 0xFFFF].reward;
                    reward += Tables.RowShiftRightTable[(blocks >> 16) & 0xFFFF].reward;
                    reward += Tables.RowShiftRightTable[(blocks >> 32) & 0xFFFF].reward;
                    reward += Tables.RowShiftRightTable[(blocks >> 48) & 0xFFFF].reward;
                    return ret;
                default:
                    return blocks;
            }
        }
        public bool MoveCheck(Direction direction)
        {
            int reward;
            return blocks != Move(direction, out reward);
        }
        public int TileCount(int tile)
        {
            int result = 0;
            for (int i = 0; i < 64; i += 4)
            {
                if (Tables.TileScoreTable[((blocks >> i) & 0xf)] == tile)
                {
                    result++;
                }
            }
            return result;
        }
        public int IndexOfTile(int tile)
        {
            for (int i = 0; i < 64; i += 4)
            {
                if (Tables.TileScoreTable[((blocks >> i) & 0xf)] == tile)
                {
                    return i / 4;
                }
            }
            return -1;
        }
        public Board[] GetRotatedBoards()
        {
            Board[] result = new Board[4];

            ushort[] rows = new ushort[4];
            ushort[] reverseRows = new ushort[4];
            ushort[] oRows = new ushort[4];
            ushort[] oReverseRows = new ushort[4];

            for (int i = 0; i < 4; i++)
            {
                rows[i] = Board.GetRow(blocks, i);
                oRows[3 - i] = rows[i];
                reverseRows[i] = Board.ReverseRow(rows[i]);
                oReverseRows[3 - i] = reverseRows[i];
            }

            result[0] = new Board(blocks);
            result[1] = new Board(Board.SetRows(oReverseRows));
            result[2] = new Board(Board.SetColumns(reverseRows));
            result[3] = new Board(Board.SetColumns(oRows));


            return result;
        }
    }
}
