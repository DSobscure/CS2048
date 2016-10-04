using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        float learningRate;
        public TupleNetwork tupleNetwork;
        //List<State> boardChain;

        int winCount;


        public TD(float learningRate)
        {
            this.learningRate = learningRate;
            //boardChain = new List<State>();
            tupleNetwork = new TupleNetwork();
            LoadNetwork();
        }
        public float PlayGame(out int maxTile, out int localStep, out Board board, bool useMLP)
        {
            float score = 0;
            Game game = new Game();
            localStep = 0;
            ulong before = 0;
            ulong prebefore = 0;
            ulong preprebefore = 0;
            List<Tuple<Board, Direction>> traningData = new List<Tuple<Board, Direction>>();
            while (!game.IsEnd)
            {
                Direction nextDirection = Direction.No;

                if(useMLP)
                {
                    nextDirection = MLP2048AI.GetDirection(game.board);
                    //Console.WriteLine("MLP Step: {0}, Score: {1}, Direction: {2}", localStep, score, nextDirection);
                }
                else
                {
                    nextDirection = FindBestMove(game.board);
                    if(game.board.MaxTile <= 1024)
                    {
                        traningData.Add(new Tuple<Board, Direction>(new Board(game.board.blocks), nextDirection));
                        //Board[] boards = game.board.GetRotatedBoards();
                        //for (int i = 0; i < 4; i++)
                        //{
                        //    traningData.Add(new Tuple<Board, Direction>(boards[i], (Direction)(((int)nextDirection - 1 + i) % 4 + 1)));
                        //}
                    }
                }

                localStep++;
                int reward;
                preprebefore = prebefore;
                prebefore = before;
                before = game.board.blocks;
                ulong blocksAfter = game.board.Move(nextDirection, out reward);
                Board boardAfter = new Board(blocksAfter);
                if (!boardAfter.IsFull)
                    boardAfter.InsertNewTile();
                ulong blocksAfterAdded = boardAfter.blocks;
                game.board.blocks = blocksAfterAdded;
                game.score += reward;

                //State state;
                //state.movedBlocks = blocksAfter;
                //state.addedBlocks = blocksAfterAdded;
                //boardChain.Add(state);

                score += reward;
                maxTile = boardAfter.MaxTile;
            }
            if(!useMLP)
            {
                before = 0;
                double errorCount = 0;
                for (int i = 0; i < traningData.Count; i++)
                {
                    double error;
                    if (MLP2048AI.GetDirection(traningData[i].Item1) != traningData[i].Item2)
                    {
                        MLP2048AI.Train(traningData[i].Item1, traningData[i].Item2, out error);
                    }
                    preprebefore = prebefore;
                    prebefore = before;
                    before = traningData[i].Item1.blocks;

                    //if (MLP2048AI.GetDirection(traningData[i].Item1) != traningData[i].Item2)
                    //{
                    //    errorCount += 1; ;
                    //}
                }
                //Console.WriteLine("Error rate: {0}", errorCount / traningData.Count);
                //Console.WriteLine("Error: {0}", totalError / traningData.Count);
                //while (errorCount / traningData.Count > 0.1)
                //{
                //    Console.WriteLine("Error rate: {0}", errorCount / traningData.Count);
                //    errorCount = 0;
                //    for (int i = 0; i < traningData.Count; i++)
                //    {
                //        if (MLP2048AI.GetDirection(traningData[i].Item1) != traningData[i].Item2)
                //        {
                //            double error;
                //            MLP2048AI.Train(traningData[i].Item1, traningData[i].Item2, out error);
                //        }
                //        if (MLP2048AI.GetDirection(traningData[i].Item1) != traningData[i].Item2)
                //        {
                //            errorCount += 1; ;
                //        }
                //    }
                //}
                //Console.WriteLine("End Loop");
            }
            //if (boardChain.Count != 0)
            //{
            //    //UpdateEvaluation();
            //    boardChain.Clear();
            //}
            if (game.board.MaxTile >= 64)
                winCount++;
            maxTile = game.board.MaxTile;
            board = game.board;
            return score;
        }
        public void PlayNGames(int times)
        {
            float maxScore = 0;
            float minScore = 1000000;
            float avgScore = 0;
            int globalMaxTile = 0;
            int globalMinTile = 65536;
            int maxCount = 0;
            int minCount = 0;
            int maxStep = 0;
            int minStep = 1000000;
            winCount = 0;
            Board minBoard = new Board();
            Board maxBoard = new Board();
            float totalSecond = 0;
            int totalSteps = 0;
            List<float> scores = new List<float>();
            using (StreamWriter recordWriter = File.AppendText("12Feature TD"))
            {
                //recordWriter.WriteLine("Combine7ModifiedGoodTuple");
                //recordWriter.WriteLine();
                //recordWriter.WriteLine("ooox");
                //recordWriter.WriteLine("oxox");
                //recordWriter.WriteLine("ooxx");
                //recordWriter.WriteLine("xxxx");
                //recordWriter.WriteLine();
                for (int i = 1; i <= times; i++)
                {
                    int localMaxTile, localStep;
                    Board b;
                    DateTime startTime = DateTime.Now;
                    float result = PlayGame(out localMaxTile, out localStep, out b, false);
                    totalSecond += Convert.ToSingle((DateTime.Now - startTime).TotalSeconds);
                    totalSteps += localStep;
                    avgScore += result;
                    scores.Add(result);
                    
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
                    int recordNumber = 10;
                    if (i % recordNumber == 0)
                    {
                        //scores.Sort();
                        //foreach (float f in scores)
                        //{
                        //    Console.WriteLine(f);
                        //}
                        double deviation = Math.Sqrt(scores.Sum(x => Math.Pow(x - avgScore / recordNumber, 2)) / recordNumber);
                        //recordWriter.WriteLine("Round: {0} AvgScore: {1}", i, avgScore / recordNumber);
                        //recordWriter.WriteLine("Max Score: {0}", maxScore);
                        //recordWriter.WriteLine("Min Score: {0}", minScore);
                        //recordWriter.WriteLine("Max Steps: {0}", maxStep);
                        //recordWriter.WriteLine("Min Steps: {0}", minStep);
                        //recordWriter.WriteLine("Win Rate: {0}", winCount * 1.0 / recordNumber);
                        //recordWriter.WriteLine("Max Tile: {0} #{1}", globalMaxTile, maxCount);
                        //recordWriter.WriteLine("Min Tile: {0} #{1}", globalMinTile, minCount);
                        //recordWriter.WriteLine("標準差: {0}", deviation);
                        //recordWriter.WriteLine("Delta Time: {0} seconds", totalSecond);
                        //recordWriter.WriteLine("Average Speed: {0}moves/sec", totalSteps / totalSecond);

                        //Console.WriteLine();
                        //Console.WriteLine("Round: {0} AvgScore: {1}", i, avgScore / recordNumber);
                        //Console.WriteLine("Max Score: {0}", maxScore);
                        //Console.WriteLine("Min Score: {0}", minScore);
                        //Console.WriteLine("Max Steps: {0}", maxStep);
                        //Console.WriteLine("Min Steps: {0}", minStep);
                        //Console.WriteLine("Win Rate: {0}", winCount * 1.0 / recordNumber);
                        //Console.WriteLine("Max Tile: {0} #{1}", globalMaxTile, maxCount);
                        //Console.WriteLine("Min Tile: {0} #{1}", globalMinTile, minCount);
                        //Console.WriteLine("標準差: {0}", deviation);
                        //Console.WriteLine("Delta Time: {0} seconds", totalSecond);
                        //Console.WriteLine("Average Speed: {0}moves/sec", totalSteps / totalSecond);
                        
                        //Console.WriteLine();
                        //minBoard.Print();
                        //maxBoard.Print();
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
                        totalSecond = 0;
                        totalSteps = 0;
                        scores.Clear();
                        float MLPscore = 0;
                        int MLPMax = 0;
                        for(int j = 0; j < 100; j++)
                        {
                            MLPscore += PlayGame(out localMaxTile, out localStep, out b, true);
                            if(localMaxTile > MLPMax)
                            {
                                MLPMax = localMaxTile;
                            }
                        }
                        Console.WriteLine("MLP100 average: {0}, Max: {1}", MLPscore/100, MLPMax);
                        //Console.ReadLine();
                    }
                    //if (i % 50000 == 0)
                    //{
                    //    SaveNetwork();
                    //}
                    //if (i % 100000 == 0)
                    //{
                    //    learningRate *= 0.95f;
                    //}
                }
                SaveNetwork();
            }
        }
        public Direction FindBestMove(Board board)
        {
            Direction nextDirection = Direction.No;
            float maxScore = -1000000.0f;
            bool isFirst = true;
            int emptyCount = board.EmptyCount;
            int maxDepth = 0;
            //if (board.MaxTile <= 2048)
            //    maxDepth = 4;
            if (emptyCount < maxDepth)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Board searchingBoard = new Board(board.blocks);
                    float result = 0;
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
            float maxScore = -1000000.0f;
            bool isFirst = true;
            for (int i = 1; i <= 4; i++)
            {
                float result = Evaluate(board, (Direction)i);
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
        public float Evaluate(Board board, Direction direction)
        {
            if(board.MoveCheck(direction))
            {
                int result;
                ulong boardAfter = board.Move(direction, out result);
                Board afterBoard = new Board(boardAfter);
                return result + tupleNetwork.GetValue(boardAfter);
            }
            else
            {
                return 0;
            }
        }
        //public void UpdateEvaluation()
        //{
        //    List<BestMoveNode> bestMoveNodes = new List<BestMoveNode>();
        //    for (int i = 0; i < boardChain.Count; i++)
        //    {
        //        Board board = new Board(boardChain[i].addedBlocks);
        //        Direction nextDirection = FindBestMove(board);
        //        int nextReward = 0;
        //        ulong movedBlocks = board.blocks;
        //        if (nextDirection != Direction.No)
        //            movedBlocks = board.Move(nextDirection, out nextReward);
        //        BestMoveNode bestMoveNode;
        //        bestMoveNode.bestMove = nextDirection;
        //        bestMoveNode.reward = nextReward;
        //        bestMoveNode.movedBlocks = movedBlocks;
        //        bestMoveNodes.Add(bestMoveNode);
        //    }
        //    for (int i = boardChain.Count - 1; i >= 0; i--)
        //    {
        //        float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(bestMoveNodes[i].movedBlocks);
        //        if (i == boardChain.Count - 1)
        //        {
        //            score = 0;
        //        }
        //        tupleNetwork.UpdateValue(boardChain[i].movedBlocks, learningRate * (score - tupleNetwork.GetValue(boardChain[i].movedBlocks)));
        //    }
        //}
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
