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
            //Console.WriteLine(Board.GetScore(0xfedc000000000000));
            //Console.WriteLine(Board.GetScore(0xefdc000000000000));
            //Console.WriteLine(Board.GetScore(0xefdc143214321432));
            //Console.WriteLine(Board.GetScore(0xefdc111111111111));
            //Console.WriteLine(Board.GetScore(0x000000000000fedc));
            //Console.WriteLine(Board.GetScore(0x000000000000cdef));
            //Console.WriteLine(Board.GetScore(0x000000000000cdfe));
            //Console.WriteLine(Board.GetScore(0x0000000000001357));
            Console.Read();
        }
    }
}
