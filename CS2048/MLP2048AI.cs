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
        private static MultiLayerPerceptron multiLayerPerceptron;

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
            //int[] nodeNumbers = new int[1];
            //for(int i = 0; i < 1; i++)
            //{
            //    nodeNumbers[i] = 20;
            //}
            multiLayerPerceptron = new MultiLayerPerceptron(16, 4, 2, new int[] {42,7 }, 0.01, activationFunction, dActivationFunction);
        }

        public static void Train(Board board, Direction desiredDirection, out double error)
        {
            double maxTile = Math.Log(board.MaxTile) / Math.Log(2);
            if (desiredDirection != Direction.No)
            {
                double[] inputBlocks = new double[16];
                int reward1, reward2, reward3, reward4;
                board.Move(Direction.Up, out reward1);
                board.Move(Direction.Down, out reward2);
                board.Move(Direction.Left, out reward3);
                board.Move(Direction.Right, out reward4);
                for (int i = 0; i < 16; i++)
                {
                    inputBlocks[i] = ((board.blocks >> (i * 4)) & 0xf) << (i * 4);
                }
                double[] output = directionTable[(int)desiredDirection - 1];
                multiLayerPerceptron.Tranning(inputBlocks, output, out error);
            }
            else
            {
                error = 0;
            }
        }

        public static Direction GetDirection(Board board)
        {
            double maxTile = Math.Log(board.MaxTile) / Math.Log(2);

            double[] inputBlocks = new double[16];
            int reward1, reward2, reward3, reward4;
            board.Move(Direction.Up, out reward1);
            board.Move(Direction.Down, out reward2);
            board.Move(Direction.Left, out reward3);
            board.Move(Direction.Right, out reward4);
            for (int i = 0; i < 16; i++)
            {
                inputBlocks[i] = ((board.blocks >> (i * 4)) & 0xf) << (i * 4);
            }
            double[] output = multiLayerPerceptron.Compute(inputBlocks);
            List<Tuple<Direction, double>> directionRank = new List<Tuple<Direction, double>>();

            for(int i = 0; i < 4; i++)
            {
                directionRank.Add(new Tuple<Direction, double>((Direction)(i+1), output[i]));
            }
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
