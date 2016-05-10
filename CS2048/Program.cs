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
            //Board board = new Board();
            //board.Initial();
            //board.Print();
            //while(board.CanMove)
            //{
            //    int reward = 0;
            //    switch (Console.ReadLine())
            //    {
            //        case "w":
            //            board.blocks = board.Move(Direction.Up, out reward);
            //            break;
            //        case "s":
            //            board.blocks = board.Move(Direction.Down, out reward);
            //            break;
            //        case "a":
            //            board.blocks = board.Move(Direction.Left, out reward);
            //            break;
            //        case "d":
            //            board.blocks = board.Move(Direction.Right, out reward);
            //            break;
            //    }
            //    board.score += reward;
            //    board.InsertNewTile();
            //    board.Print();
            //    Console.WriteLine(Board.GetScore(board.blocks));
            //}
            TDLearningAgent agent = new TDLearningAgent();
            //foreach (Feature f in agent.td.tupleNetwork.featureSet)
            //{
            //    //Console.WriteLine("{0}/{1}", f.tuples.Count(x => x == 0), f.tuples.Length);
            //    Console.WriteLine((f as DictionaryFeature).dictionaryTuple.Count);
            //}
            agent.Training(500000);
            //foreach(var pair in (agent.td.tupleNetwork.featureSet[0]as DictionaryFeature).dictionaryTuple)
            //{
            //    Console.WriteLine("index: {0}   score:{1}", pair.Key, pair.Value);
            //}
            Console.Read();
        }
    }
}
