using CS2048.NeuralNetworkLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS2048
{
    public class MLP2048AI
    {
        static List<double[]> directionTable = new List<double[]>
        {
            new double[4] { 1, 0, 0, 0 },
            new double[4] { 0, 1, 0, 0 },
            new double[4] { 0, 0, 1, 0 },
            new double[4] { 0, 0, 0, 1 },
        };
        private static MultiLayerPerceptron upStrategy;
        private static MultiLayerPerceptron downStrategy;
        private static MultiLayerPerceptron leftStrategy;
        private static MultiLayerPerceptron rightStrategy;

        static MLP2048AI()
        {
            Func<double, double> activationFunction = (input) =>
            {
                return 1.0 / (1.0 + Math.Exp(-input));
            };
            Func<double, double> dActivationFunction = (input) =>
            {
                return activationFunction(input) * (1 - activationFunction(input));
            };
            upStrategy = new MultiLayerPerceptron(32, 1, 3, new int[] { 500, 100, 7 }, 0.1, activationFunction, dActivationFunction);
            downStrategy = new MultiLayerPerceptron(32, 1, 3, new int[] { 500, 100, 7 }, 0.1, activationFunction, dActivationFunction);
            leftStrategy = new MultiLayerPerceptron(32, 1, 3, new int[] { 500, 100, 7 }, 0.1, activationFunction, dActivationFunction);
            rightStrategy = new MultiLayerPerceptron(32, 1, 3, new int[] { 500, 100, 7 }, 0.1, activationFunction, dActivationFunction);
        }

        public static void Train(Board board, Direction desiredDirection, out double error)
        {
            if (desiredDirection != Direction.No)
            {
                double[] inputBlocks = new double[32];
                int reward1, reward2, reward3, reward4;
                ulong upBoard = board.Move(Direction.Up, out reward1);
                ulong downBoard = board.Move(Direction.Down, out reward2);
                ulong leftBoard = board.Move(Direction.Left, out reward3);
                ulong rightBoard = board.Move(Direction.Right, out reward4);
                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i] = ((board.blocks >> (i * 4)) & 0xf);
                }
                double[] output = directionTable[(int)desiredDirection - 1];

                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i + 16] = ((upBoard >> (i * 4)) & 0xf);
                }
                upStrategy.Tranning(inputBlocks, new double[] { output[0] }, out error);
                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i + 16] = ((downBoard >> (i * 4)) & 0xf);
                }
                downStrategy.Tranning(inputBlocks, new double[] { output[1] }, out error);
                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i + 16] = ((leftBoard >> (i * 4)) & 0xf);
                }
                leftStrategy.Tranning(inputBlocks, new double[] { output[2] }, out error);
                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i + 16] = ((rightBoard >> (i * 4)) & 0xf);
                }
                rightStrategy.Tranning(inputBlocks, new double[] { output[3] }, out error);
            }
            else
            {
                error = 0;
            }
        }

        public static Direction GetDirection(Board board)
        {
            double maxTile = Math.Log(board.MaxTile) / Math.Log(2);

            double[] inputBlocks = new double[32];
            int reward1, reward2, reward3, reward4;
            ulong upBoard = board.Move(Direction.Up, out reward1);
            ulong downBoard = board.Move(Direction.Down, out reward2);
            ulong leftBoard = board.Move(Direction.Left, out reward3);
            ulong rightBoard = board.Move(Direction.Right, out reward4);
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i] = ((board.blocks >> (i * 4)) & 0xf);
            }
            List<Tuple<Direction, double>> directionRank = new List<Tuple<Direction, double>>();
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i + 16] = ((upBoard >> (i * 4)) & 0xf);
            }
            double upScore = upStrategy.Compute(inputBlocks)[0];
            directionRank.Add(new Tuple<Direction, double>(Direction.Up, upScore));
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i + 16] = ((downBoard >> (i * 4)) & 0xf);
            }
            double downScore = downStrategy.Compute(inputBlocks)[0];
            directionRank.Add(new Tuple<Direction, double>(Direction.Down, downScore));
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i + 16] = ((leftBoard >> (i * 4)) & 0xf);
            }
            double leftScore = leftStrategy.Compute(inputBlocks)[0];
            directionRank.Add(new Tuple<Direction, double>(Direction.Left, leftScore));
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i + 16] = ((rightBoard >> (i * 4)) & 0xf);
            }
            double rightScore = rightStrategy.Compute(inputBlocks)[0];
            directionRank.Add(new Tuple<Direction, double>(Direction.Right, rightScore));

            directionRank = directionRank.OrderBy(x => x.Item2).ToList();
            for (int i = 3; i >= 0; i--)
            {
                if(board.MoveCheck(directionRank[i].Item1))
                {
                    return directionRank[i].Item1;
                }
            }

            return Direction.No;
        }
    }
}
