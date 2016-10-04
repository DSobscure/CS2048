using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace CS2048
{
    public class TupleNetwork
    {
        [JsonProperty("featureSet")]
        public List<Feature> featureSet;
        ulong[] rotateBoards;
        ulong[] isomorphicBoards;

        [JsonConstructor]
        public TupleNetwork(List<Feature> featureSet)
        {
            this.featureSet = featureSet;
            rotateBoards = new ulong[4];
            isomorphicBoards = new ulong[8];
        }
        public TupleNetwork()
        {
            featureSet = new List<Feature>();

            featureSet.Add(new FourTupleFeature(3));
            featureSet.Add(new FourTupleFeature(7));
            featureSet.Add(new FiveTupleFeature(3));
            featureSet.Add(new FiveTupleFeature(2));
            featureSet.Add(new FiveTupleFeature(7));
            featureSet.Add(new FiveTupleFeature(5));
            featureSet.Add(new SixTupleFeature(2));
            featureSet.Add(new SixTupleFeature(7));
            featureSet.Add(new SixTupleFeature(3));
            featureSet.Add(new SixTupleFeature(1));
            featureSet.Add(new SixTupleFeature(6));
            featureSet.Add(new SixTupleFeature(5));

            rotateBoards = new ulong[4];
            isomorphicBoards = new ulong[8];
        }
        public float GetValue(ulong blocks)
        {
            SetSymmetricBoards(blocks);
            float sum = 0;
            for (int i = 0; i < featureSet.Count; i++)
            {
                sum += featureSet[i].GetScore(blocks);
            }
            return sum;
        }
        public void UpdateValue(ulong blocks, float delta)
        {
            SetSymmetricBoards(blocks);
            for (int i = 0; i < featureSet.Count; i++)
            {
                featureSet[i].UpdateScore(blocks, delta);
            }
        }
        public void SetSymmetricBoards(ulong blocks)
        {
		    ushort[] rows = new ushort[4];
            ushort[] reverseRows = new ushort[4];
            ushort[] oRows = new ushort[4];
            ushort[] oReverseRows = new ushort[4];

            for (int i = 0; i< 4; i++)
            {
			    rows[i] = Board.GetRow(blocks, i);
			    oRows[3 - i] = rows[i];
			    reverseRows[i] = Board.ReverseRow(rows[i]);
			    oReverseRows[3 - i] = reverseRows[i];
		    }

            rotateBoards[0] = blocks;
            rotateBoards[1] = Board.SetColumns(reverseRows);
            rotateBoards[2] = Board.SetColumns(oRows);
            rotateBoards[3] = Board.SetRows(oReverseRows);

            isomorphicBoards[0] = blocks;
            isomorphicBoards[1] = Board.SetRows(reverseRows);
            isomorphicBoards[2] = Board.SetRows(oRows);
            isomorphicBoards[3] = Board.SetRows(oReverseRows);
            isomorphicBoards[4] = Board.SetColumns(rows);
            isomorphicBoards[5] = Board.SetColumns(reverseRows);
            isomorphicBoards[6] = Board.SetColumns(oRows);
            isomorphicBoards[7] = Board.SetColumns(oReverseRows);

            for (int i = 0; i<featureSet.Count; i++)
            {
		        featureSet[i].SetSymmetricBoards(rotateBoards, isomorphicBoards);
            }
	    }
        public void Save()
        {
            for(int i = 0; i < featureSet.Count; i++)
            {
                string result = JsonConvert.SerializeObject(featureSet[i], new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                File.WriteAllText("data"+i.ToString(), result);
            }
        }
        public void Load()
        {
            for (int i = 0; i < featureSet.Count; i++)
            {
                if (File.Exists("data" + i.ToString()))
                {
                    string result = File.ReadAllText("data" + i.ToString());
                    if(featureSet[i] is SpecialFeature)
                        featureSet[i] = (JsonConvert.DeserializeObject<SpecialFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                    else if(featureSet[i] is DictionaryFeature)
                        featureSet[i] = (JsonConvert.DeserializeObject<DictionaryFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                    else if(featureSet[i] is FourTupleFeature)
                        featureSet[i] = (JsonConvert.DeserializeObject<FourTupleFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                    else if (featureSet[i] is FiveTupleFeature)
                        featureSet[i] = (JsonConvert.DeserializeObject<FiveTupleFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                    else if (featureSet[i] is SixTupleFeature)
                        featureSet[i] = (JsonConvert.DeserializeObject<SixTupleFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                }
            }
        }

    }
}
