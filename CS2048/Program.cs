﻿using System;
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
            agent.Training(10000);
            Console.Read();
        }
    }
}
