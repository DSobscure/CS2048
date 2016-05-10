using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace CS2048
{
    public struct State
    {
        public ulong movedBlocks;
        public ulong addedBlocks;
    }
    public struct BestMoveNode
    {
	    public Direction bestMove;
        public int reward;
        public ulong movedBlocks;
    };

    public class TD
    {
        double learningRate;
        public TupleNetwork tupleNetwork;
        List<State> boardChain;

        int winCount;
        int numberOfBoardsCollected;
        double lambda;

        public TD(double learningRate)
        {
            this.learningRate = learningRate;
            boardChain = new List<State>();
            tupleNetwork = new TupleNetwork();
            LoadNetwork();
        }
        public double PlayGame(out int maxTile, out int localStep, out Board board)
        {
            double score = 0.0;
            Game game = new Game();
            bool isFirst16384 = true;
            localStep = 0;
            while (!game.IsEnd)
            {
                //game.board.Print();
                //Console.Read();
                Direction nextDirection = Direction.No;

                nextDirection = FindBestMove(game.board);
                localStep++;
                int reward;
                ulong blocksAfter = game.board.Move(nextDirection, out reward);
                Board boardAfter = new Board(blocksAfter);
                if (!boardAfter.IsFull)
                    boardAfter.InsertNewTile();
                ulong blocksAfterAdded = boardAfter.blocks;
                game.board.blocks = blocksAfterAdded;
                game.score += reward;

                State state;
                state.movedBlocks = blocksAfter;
                state.addedBlocks = blocksAfterAdded;
                boardChain.Add(state);

                score += reward;
                maxTile = boardAfter.MaxTile;
                bool is8192Appear = maxTile == 8192;
                bool is16384Appear = maxTile == 16384;
                if (is16384Appear && isFirst16384)
                {
                    Console.WriteLine("First 16384 blocks: {0}, score: {1}", game.board.blocks, game.score);
                    numberOfBoardsCollected++;
                    isFirst16384 = false;
                    break;
                }
            }
            if (boardChain.Count != 0)
            {
                UpdateEvaluation();
                boardChain.Clear();
            }
            if (game.board.MaxTile >= 2048)
                winCount++;
            maxTile = game.board.MaxTile;
            //Console.WriteLine("max:{0} repeated:{1}",maxTile,game.board.RepeatedTileCount);
            //game.board.Print();
            board = game.board;
            return score;
        }
        public void PlayNGames(int times)
        {
            double maxScore = 0.0;
            double minScore = 1000000;
            double avgScore = 0.0;
            int globalMaxTile = 0;
            int globalMinTile = 65536;
            int maxCount = 0;
            int minCount = 0;
            int maxStep = 0;
            int minStep = 1000000;
            winCount = 0;
            Board minBoard = new Board();
            Board maxBoard = new Board();

            for (int i = 1; i <= times; i++)
            {
                int localMaxTile,localStep;
                Board b;
                double result = PlayGame(out localMaxTile, out localStep, out b);
                avgScore += result;

                if (result > maxScore)
                {
                    maxScore = result;
                    maxBoard = b;
                }
                if (result < minScore)
                {
                    minScore = result;
                    minBoard = b;
                }
                    
                if (localStep > maxStep)
                    maxStep = localStep;
                if (localStep < minStep)
                    minStep = localStep;
                if (localMaxTile > globalMaxTile)
                {
                    globalMaxTile = localMaxTile;
                    maxCount = 1;
                }
                else if (globalMaxTile == localMaxTile)
                    maxCount++;
                if (localMaxTile < globalMinTile)
                {
                    globalMinTile = localMaxTile;
                    minCount = 1;
                }
                else if (globalMinTile == localMaxTile)
                    minCount++;

                if (i % 100 == 0)
                {
                    Console.WriteLine("Round: {0} AvgScore: {1}", i, avgScore / 100);
                    Console.WriteLine("Max Score: {0}", maxScore);
                    Console.WriteLine("Min Score: {0}", minScore);
                    Console.WriteLine("Max Steps: {0}", maxStep);
                    Console.WriteLine("Min Steps: {0}", minStep);
                    Console.WriteLine("Win Rate: {0}", winCount * 1.0 / 100);
                    Console.WriteLine("Max Tile: {0} #{1}", globalMaxTile, maxCount);
                    Console.WriteLine("Min Tile: {0} #{1}", globalMinTile, minCount);
                    Console.WriteLine();
                    minBoard.Print();
                    maxBoard.Print();
                    avgScore = 0;
                    winCount = 0;
                    maxScore = 0;
                    minScore = 1000000;
                    globalMaxTile = 0;
                    globalMinTile = 65536;
                    maxCount = 0;
                    minCount = 0;
                    maxStep = 0;
                    minStep = 1000000;
                }
                if(i % 1000 == 0)
                {
                    SaveNetwork();
                }
            }
            SaveNetwork();
        }
        public Direction FindBestMove(Board board)
        {
            Direction nextDirection = Direction.No;
            double maxScore = -1000000.0;
            bool isFirst = true;
            int emptyCount = board.EmptyCount;
            int maxDepth = 2;
            if (board.MaxTile <= 2048)
                maxDepth = 4;
            if (emptyCount < maxDepth)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Board searchingBoard = new Board(board.blocks);
                    double result = 0;
                    if (searchingBoard.MoveCheck((Direction)i))
                    {
                        Direction predictedDirection = (Direction)i;
                        int reward = 0;
                        for (int searchDepth = 0; searchDepth < maxDepth - emptyCount && searchingBoard.CanMove; searchDepth++)
                        {
                            result += Evaluate(searchingBoard, predictedDirection);
                            searchingBoard.blocks = searchingBoard.Move(predictedDirection, out reward);
                            searchingBoard.InsertNewTile();
                            predictedDirection = GetBestMoveIn1Step(searchingBoard);
                        }
                        if (searchingBoard.CanMove)
                            result += 10000;
                        if (isFirst)
                        {
                            nextDirection = (Direction)i;
                            maxScore = result;
                            isFirst = false;
                        }
                        else if (result > maxScore)
                        {
                            nextDirection = (Direction)i;
                            maxScore = result;
                        }
                    }
                }
            }
            else
            {
                nextDirection = GetBestMoveIn1Step(board);
            }
            return nextDirection;
        }
        public Direction GetBestMoveIn1Step(Board board)
        {
            Direction nextDirection = Direction.No;
            double maxScore = -1000000.0;
            bool isFirst = true;
            for (int i = 1; i <= 4; i++)
            {
                double result = Evaluate(board, (Direction)i);

                if (board.MoveCheck((Direction)i) && isFirst)
                {
                    nextDirection = (Direction)i;
                    maxScore = result;
                    isFirst = false;
                }
                if (board.MoveCheck((Direction)i) && result > maxScore)
                {
                    nextDirection = (Direction)i;
                    maxScore = result;
                }
            }
            return nextDirection;
        }
        public double Evaluate(Board board, Direction direction)
        {
            if(board.MoveCheck(direction))
            {
                int result;
                ulong boardAfter = board.Move(direction, out result);
                Board afterBoard = new Board(boardAfter);
                return result/( 1.0 + afterBoard.RepeatedTileCount) + tupleNetwork.GetValue(boardAfter);
            }
            else
            {
                return 0;
            }
        }
        public void UpdateEvaluation()
        {
            List<BestMoveNode> bestMoveNodes = new List<BestMoveNode>();

            for (int i = 0; i < boardChain.Count; i++)
            {
                Board board = new Board(boardChain[i].addedBlocks);
                Direction nextDirection = FindBestMove(board);
                int nextReward = 0;
                ulong movedBlocks = board.blocks;
                if (nextDirection != Direction.No)
                    movedBlocks = board.Move(nextDirection, out nextReward);
                BestMoveNode bestMoveNode;
                bestMoveNode.bestMove = nextDirection;
                bestMoveNode.reward = nextReward;
                bestMoveNode.movedBlocks = movedBlocks;
                bestMoveNodes.Add(bestMoveNode);
            }

            lambda = 0.25;
            for (int i = boardChain.Count - 1; i >= 0; i--)
            {

                int size = 5;
                double score = 0;
                int totalReward = 0;
                for (int j = 0; j < size && (i + j) < boardChain.Count; j++)
                {

                    double weight = 0;
                    if (j != size - 1)
                    {
                        weight = (1 - lambda) * Math.Pow(lambda, j * 1.0);
                    }
                    else
                    {
                        weight = Math.Pow(lambda, j * 1.0);
                    }

                    totalReward += bestMoveNodes[i].reward;
                    score += weight * (totalReward + tupleNetwork.GetValue(bestMoveNodes[i].movedBlocks));
                }
                tupleNetwork.UpdateValue(boardChain[i].movedBlocks, learningRate * (score - tupleNetwork.GetValue(boardChain[i].movedBlocks)));
            }
        }
        public void SaveNetwork()
        {
            tupleNetwork.Save();
        }
        public void LoadNetwork()
        {
            tupleNetwork.Load();
        }
    }
}
