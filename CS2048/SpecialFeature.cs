using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS2048
{
    public class SpecialFeature : Feature
    {
        [JsonProperty("index")]
        int index;

        [JsonConstructor]
        public SpecialFeature(double[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public SpecialFeature(int index)
        {
            this.index = index;
            switch(index)
            {
                case 1:
                case 2:
                case 3:
                case 6:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    tuples = new double[1048576];
                    break;
                case 4:
                case 5:
                case 7:
                    tuples = new double[65536];
                    break;
            }
            tuples = new double[1048576];
        }

        public override int GetIndex(ulong blocks)
        {
            switch (index)
            {
                case 1:
                    //ooox
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 2:
                    //xoox
                    //ooxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 28) & 0xF));
                case 3:
                    //xxxx
                    //xxoo
                    //ooox
                    //xxxx
                    return (int)(((blocks >> 20) & 0xF0000) | ((blocks >> 20) & 0xF000) | ((blocks >> 20) & 0xF00) | ((blocks >> 20) & 0xF0) | ((blocks >> 20) & 0xF));
                case 4:
                    //oxxx
                    //oxxx
                    //oxxx
                    //oxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 12) & 0xF));
                case 5:
                    //ooox
                    //oxxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 48) & 0xF0) | ((blocks >> 44) & 0xF));
                case 6:
                    //xxxo
                    //xxxo
                    //xooo
                    //xxxx
                    return (int)(((blocks >>32) & 0xF0000) | ((blocks >> 20) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 16) & 0xF));
                case 7:
                    //xxxx
                    //xxxx
                    //xxxo
                    //xooo
                    return (int)(((blocks >> 4) & 0xF000) | ((blocks >> 0) & 0xF00) | ((blocks >> 0) & 0xF0) | ((blocks >> 0) & 0xF));
                case 8:
                    //oxoo
                    //xxxx
                    //ooxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 24) & 0xF));
                case 9:
                    //xxxx
                    //xoxx
                    //ooox
                    //xoxx
                    return (int)(((blocks >> 24) & 0xF0000) | ((blocks >> 16) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 8) & 0xF));
                case 10:
                    //xxxx
                    //oxxx
                    //ooox
                    //oxxx
                    return (int)(((blocks >> 28) & 0xF0000) | ((blocks >> 16) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 12) & 0xF));
                case 11:
                    //ooox
                    //oxxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 28) & 0xF));
                case 12:
                    //xoox
                    //oxxx
                    //oxxx
                    //oxxx
                    return (int)(((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 12) & 0xF));
                case 13:
                    //oxox
                    //xoxx
                    //oxox
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 20) & 0xF));
                default:
                    return 0;
            }
        }
    }
}
