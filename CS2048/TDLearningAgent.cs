using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2048
{
    public class TDLearningAgent
    {
        public TD td;

        public TDLearningAgent()
        {
            td = new TD(0.0025f);
        }

        public void Training(int trainingTimes)
        {
            td.PlayNGames(trainingTimes);
        }
    }
}
