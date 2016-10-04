using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public struct RowShiftInfo
    {
        public ushort row;
        public int reward;
    }
    public struct ColumnShiftInfo
    {
        public ulong column;
        public int reward;
    }
    static public class Tables
    {
        public static readonly float CommonRatio = 1f;
        public static float[] CommonRatioTable = new float[16];
        public static int[] TileScoreTable = new int[16];
        public static float[,] LineScoreTable = new float[65536, 4];
        public static float[] MaxScoreOfSumTable = new float[65536];
        static float[] MinScoreOfSumTable = new float[65536];
        public static int[] RowSumTable = new int[65536];
        public static int[] Log2Table = new int[65536];
        public static RowShiftInfo[] RowShiftLeftTable = new RowShiftInfo[65536];
        public static RowShiftInfo[] RowShiftRightTable = new RowShiftInfo[65536];
        public static ColumnShiftInfo[] ColumnShiftUpTable = new ColumnShiftInfo[65536];
        public static ColumnShiftInfo[] ColumnShiftDownTable = new ColumnShiftInfo[65536];

        static Tables()
        {
            InitilShiftTable();
            InitialCommonRatioTable();
            InitialTileScoreTable();
            InitialLineScoreTable();
            InitialMinMaxScoreOfSumTable();
            InitialRowSumTable();
            InitialLog2Table();
        }
        static void InitilShiftTable()
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
                        reward += 2 << (lines[i] - 1);
                    }
                }

                int result = (lines[0] << 0) |
                             (lines[1] << 4) |
                             (lines[2] << 8) |
                             (lines[3] << 12);
                int rev_result = Board.ReverseRow(result);
                int rev_row = Board.ReverseRow(row);

                RowShiftLeftTable[row] = new RowShiftInfo { row = (ushort)(row ^ result), reward = reward };
                RowShiftRightTable[rev_row] = new RowShiftInfo { row = (ushort)(rev_row ^ rev_result), reward = reward };
                ColumnShiftUpTable[row] = new ColumnShiftInfo { column = Board.UnpackColumn(row) ^ Board.UnpackColumn(result), reward = reward };
                ColumnShiftDownTable[rev_row] = new ColumnShiftInfo { column = Board.UnpackColumn(rev_row) ^ Board.UnpackColumn(rev_result), reward = reward };
            }
        }
        static void InitialCommonRatioTable()
        {
            CommonRatioTable[0] = 1;
            for (int i = 1; i < 16; i++)
            {
                CommonRatioTable[i] = CommonRatioTable[i - 1] * CommonRatio;
            }
        }
        static void InitialTileScoreTable()
        {
            TileScoreTable[0] = 0;
            TileScoreTable[1] = 2;
            for (int i = 2; i < 16; i++)
            {
                TileScoreTable[i] = TileScoreTable[i - 1] * 2;
            }
        }
        static void InitialLineScoreTable()
        {
            int rowMask = 0xf;
            for (int number = 0; number < 65536; number++)
            {
                int[] blockNumbers = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    blockNumbers[i] = ((number >> (4 * i)) & rowMask);
                }

                LineScoreTable[number, 0] = TileScoreTable[blockNumbers[0]] * CommonRatioTable[0]/* * (3*blockNumbers[0] - blockNumbers[1] - blockNumbers[2] - blockNumbers[3])*/
                    + TileScoreTable[blockNumbers[1]] * CommonRatioTable[1]/* * (2*blockNumbers[1] - blockNumbers[2] - blockNumbers[3])*/
                    + TileScoreTable[blockNumbers[2]] * CommonRatioTable[2]/* * (blockNumbers[2] - blockNumbers[3])*/
                    + TileScoreTable[blockNumbers[3]] * CommonRatioTable[3];
                LineScoreTable[number, 1] = TileScoreTable[blockNumbers[0]] * CommonRatioTable[4]/* * (3 * blockNumbers[0] - blockNumbers[1] - blockNumbers[2] - blockNumbers[3])*/
                    + TileScoreTable[blockNumbers[1]] * CommonRatioTable[5]// * (2 * blockNumbers[1] - blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[2]] * CommonRatioTable[6]// * (blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[3]] * CommonRatioTable[7];
                LineScoreTable[number, 2] = TileScoreTable[blockNumbers[0]]* CommonRatioTable[8]// * (3 * blockNumbers[0] - blockNumbers[1] - blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[1]] * CommonRatioTable[9]// * (2 * blockNumbers[1] - blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[2]] * CommonRatioTable[10]// * (blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[3]] * CommonRatioTable[11];
                LineScoreTable[number, 3] = TileScoreTable[blockNumbers[0]] * CommonRatioTable[12]// * (3 * blockNumbers[0] - blockNumbers[1] - blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[1]] * CommonRatioTable[13]// * (2 * blockNumbers[1] - blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[2]] * CommonRatioTable[14]// * (blockNumbers[2] - blockNumbers[3])
                    + TileScoreTable[blockNumbers[3]] * CommonRatioTable[15];
            }
        }
        static void InitialMinMaxScoreOfSumTable()
        {
            MaxScoreOfSumTable[0] = 0;
            MinScoreOfSumTable[0] = 0;
            for (int number = 1; number < 32768; number++)
            {
                int finalSum = number * 2;
                int maxValue = 32768;
                int power = 15;
                int count = 0;
                while (true)
                {
                    int value = finalSum / maxValue;
                    if (value > 0)
                    {
                        MaxScoreOfSumTable[number * 2] += TileScoreTable[power] * CommonRatioTable[count];
                        MinScoreOfSumTable[number * 2] += TileScoreTable[power] * CommonRatioTable[15 - count];
                        finalSum -= maxValue;
                        count++;
                        if (finalSum == 0)
                            break;
                    }
                    maxValue /= 2;
                    power--;
                }
            }
        }
        static void InitialRowSumTable()
        {
            for (int number = 0; number < 65536; number++)
            {
                for (int i = 0; i < 16; i += 4)
                {
                    int blockNumber = (number >> i) & 0xf;
                    RowSumTable[number] += TileScoreTable[blockNumber];
                }
            }
        }
        static void InitialLog2Table()
        {
            int number = 1;
            for (int i = 0; i < 16; i++)
            {
                Log2Table[number] = i;
                number *= 2;
            }
        }
    }
}
