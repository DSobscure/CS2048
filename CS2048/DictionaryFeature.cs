using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS2048
{
    public class DictionaryFeature : Feature
    {
        [JsonProperty("dictionaryTuple")]
        public Dictionary<int, double> dictionaryTuple;
        [JsonProperty("index")]
        public int index;
        [JsonConstructor]
        public DictionaryFeature(int index, Dictionary<int, double> dictionaryTuple)
        {
            this.index = index;
            this.dictionaryTuple = dictionaryTuple;
        }
        public DictionaryFeature(int index)
        {
            dictionaryTuple = new Dictionary<int, double>();
            this.index = index;
        }

        public override int GetIndex(ulong blocks)
        {
            switch(index)
            {
                case 1:
                    //oooo
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 2:
                    //ooox
                    //ooxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 28) & 0xF));
                case 3:
                    //oooo
                    //ooxx
                    //ooxx
                    //xxxx
                    return (int)(((blocks >> 32) & 0xF0000000) | ((blocks >> 32) & 0xF000000) | ((blocks >> 32) & 0xF00000) | ((blocks >> 32) & 0xF0000) | ((blocks >> 32) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 24) & 0xF));
                case 4:
                    //oooo
                    //oooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 32) & 0xF0000000) | ((blocks >> 32) & 0xF000000) | ((blocks >> 32) & 0xF00000) | ((blocks >> 32) & 0xF0000) | ((blocks >> 32) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                case 5:
                    //ooox
                    //ooox
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 36) & 0xF000000) | ((blocks >> 36) & 0xF00000) | ((blocks >> 36) & 0xF0000) | ((blocks >> 32) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 28) & 0xF));
                case 6:
                    //xxxx
                    //xxxx
                    //xooo
                    //oooo
                    return (int)(((blocks) & 0xF000000) | ((blocks) & 0xF00000) | ((blocks) & 0xF0000) | ((blocks) & 0xF000) | ((blocks) & 0xF00) | ((blocks) & 0xF0) | ((blocks) & 0xF));
                case 7:
                    //ooox
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 8:
                    //xxxx
                    //oxxx
                    //ooox
                    //oxxx
                    return (int)(((blocks >> 28) & 0xF0000) | ((blocks >> 16) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 12) & 0xF));
                case 9:
                    //xxxx
                    //xxxo
                    //xxoo
                    //xooo
                    return (int)(((blocks >> 12) & 0xF00000) | ((blocks >> 4) & 0xF0000) | ((blocks >> 4) & 0xF000) | ((blocks >> 0) & 0xF00) | ((blocks >> 0) & 0xF0) | ((blocks >> 0) & 0xF));
                case 10:
                    //xxoo
                    //ooox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 36) & 0xF0000) | ((blocks >> 36) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 36) & 0xF));
                case 11:
                    //xxoo
                    //xoox
                    //ooxx
                    //xxxx
                    return (int)(((blocks >> 24) & 0xF00000) | ((blocks >> 24) & 0xF0000) | ((blocks >> 20) & 0xF000) | ((blocks >> 20) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 16) & 0xF));
                default:
                    return 0;
            }
        }
        public override double GetScore(ulong blocks)
        {
            double sum = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricBoard(rotateBoards[i]));
                if(dictionaryTuple.ContainsKey(index))
                    sum += dictionaryTuple[index];
                if (/*symmetricIndex != index &&*/ dictionaryTuple.ContainsKey(symmetricIndex))
                    sum += dictionaryTuple[symmetricIndex];
            }
            return sum;
        }
        public override void UpdateScore(ulong blocks, double delta)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricBoard(rotateBoards[i]));
                if (dictionaryTuple.ContainsKey(index))
                    dictionaryTuple[index] += delta;
                else
                    dictionaryTuple.Add(index, delta);
                if (/*symmetricIndex != index &&*/ dictionaryTuple.ContainsKey(symmetricIndex))
                    dictionaryTuple[symmetricIndex] += delta;
                else if (symmetricIndex != index)
                    dictionaryTuple.Add(symmetricIndex, delta);
            }
        }
    }
}
