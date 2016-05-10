using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS2048
{
    public abstract class Feature
    {
        [JsonProperty("tuples")]
        public double[] tuples;
        public virtual void UpdateScore(ulong blocks, double delta)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int index = GetIndex(rotateBoards[i]);
                    int symmetricIndex = GetIndex(GetMirrorSymmetricBoard(rotateBoards[i]));

                    tuples[index] += delta;
                    if (symmetricIndex != index)
                        tuples[symmetricIndex] += delta;
                }
            }
        }
        public virtual double GetScore(ulong blocks)
        {
            double sum = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricBoard(rotateBoards[i]));

                sum += tuples[index];
                if (symmetricIndex != index)
                    sum += tuples[symmetricIndex];
            }
            return sum;
        }
        public abstract int GetIndex(ulong blocks);

        public ulong[] rotateBoards;
        public ulong[] isomorphicBoards;

        [JsonConstructor]
        public Feature(double[] tuples)
        {
            this.tuples = tuples;
        }
        public Feature()
        { }
        public void SetSymmetricBoards(ulong[] rotateSymmetry, ulong[] isomorphic)
        {
            rotateBoards = rotateSymmetry;
            isomorphicBoards = isomorphic;
        }
        public ulong GetMirrorSymmetricBoard(ulong boardStatus)
        {
            ushort[] reverseRows = new ushort[4];

            for (int i = 0; i < 4; i++)
            {
                reverseRows[i] = Board.ReverseRow(Board.GetRow(boardStatus, i));
            }

            ulong b = Board.SetRows(reverseRows);
            return b;
        }
    }
}
