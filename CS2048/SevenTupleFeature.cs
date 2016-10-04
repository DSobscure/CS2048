using Newtonsoft.Json;

namespace CS2048
{
    public class SevenTupleFeature : Feature
    {
        [JsonProperty("index")]
        int index;

        [JsonConstructor]
        public SevenTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public SevenTupleFeature(int index)
        {
            this.index = index;
            tuples = new float[268435456];
        }

        public override int GetIndex(ulong blocks)
        {
            switch (index)
            {
                case 1:
                    //ooox
                    //oxox
                    //ooxx
                    //xxxx
                    return (int)(((blocks >> 36) & 0xF000000) | ((blocks >> 36) & 0xF00000) | ((blocks >> 36) & 0xF0000) | ((blocks >> 32) & 0xF000) | ((blocks >> 28) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 24) & 0xF));
                default:
                    return 0;
            }
        }
    }
}
