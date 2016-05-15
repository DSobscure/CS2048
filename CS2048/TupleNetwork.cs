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
            rotateBoards = new ulong[4];
            isomorphicBoards = new ulong[8];
            featureSet.Add(new SpecialFeature(1));//9000
            //featureSet.Add(new SpecialFeature(2));//3000
            //featureSet.Add(new SpecialFeature(3));//6000
            //featureSet.Add(new SpecialFeature(4));//2500
            //featureSet.Add(new SpecialFeature(5));//4000
            //featureSet.Add(new SpecialFeature(6));//9500
            //featureSet.Add(new SpecialFeature(7));//4000
            //featureSet.Add(new SpecialFeature(8));//2400
            //featureSet.Add(new SpecialFeature(9));//5500
            //featureSet.Add(new SpecialFeature(10));//10500
            //featureSet.Add(new SpecialFeature(11));//3000
            //featureSet.Add(new SpecialFeature(12));//2500
            //featureSet.Add(new SpecialFeature(13));
            //featureSet.Add(new DictionaryFeature(1));//8000 11000
            //featureSet.Add(new DictionaryFeature(2));//5000 10000
            //featureSet.Add(new DictionaryFeature(3));//6500 10500
            //featureSet.Add(new DictionaryFeature(4));//4000 9000
            //featureSet.Add(new DictionaryFeature(5));//6000 10000
            //featureSet.Add(new DictionaryFeature(6));//6000 10000
            //featureSet.Add(new DictionaryFeature(7));//good
            //featureSet.Add(new DictionaryFeature(8));//good
            //featureSet.Add(new DictionaryFeature(9));//
            //featureSet.Add(new DictionaryFeature(10));//normal
            //featureSet.Add(new DictionaryFeature(11));//
        }
        public double GetValue(ulong blocks)
        {
            SetSymmetricBoards(blocks);
            double sum = 0;
            for (int i = 0; i < featureSet.Count; i++)
            {
                sum += featureSet[i].GetScore(blocks);
            }
            return sum;
        }
        public void UpdateValue(ulong blocks, double delta)
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
		
		    //isomorphicBoards[0] = blocks;
		    //isomorphicBoards[1] = Board.SetRows(reverseRows);
		    //isomorphicBoards[2] = Board.SetRows(oRows);
		    //isomorphicBoards[3] = Board.SetRows(oReverseRows);
		    //isomorphicBoards[4] = Board.SetColumns(rows);
		    //isomorphicBoards[5] = Board.SetColumns(reverseRows);
		    //isomorphicBoards[6] = Board.SetColumns(oRows);
		    //isomorphicBoards[7] = Board.SetColumns(oReverseRows);
		
		    for(int i = 0; i<featureSet.Count; i++)
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
                        featureSet.Add(JsonConvert.DeserializeObject<SpecialFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                    else if(featureSet[i] is DictionaryFeature)
                        featureSet.Add(JsonConvert.DeserializeObject<DictionaryFeature>(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
                }
            }
        }

    }
}
