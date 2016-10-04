﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CS2048
{
    public class LFeature : Feature
    {
        [JsonConstructor]
        public LFeature(float[] tuples)
        {
            this.tuples = tuples;
        }
        public LFeature()
        {
            tuples = new float[16];
        }
        public override int GetIndex(ulong blocks)
        {
            return (int)(((blocks >> 44) & 0xF));
        }
    }
}
