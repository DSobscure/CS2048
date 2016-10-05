using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS2048.NeuralNetworkLibrary;

namespace CS2048
{
    class MLPBoardTest
    {
        public static void Run()
        {
            Func<double, double> activationFunction = (input) =>
            {
                return 1.0 / (1.0 + Math.Exp(-input));
            };
            Func<double, double> dActivationFunction = (input) =>
            {
                return activationFunction(input) * (1 - activationFunction(input));
            };
            MultiLayerPerceptron mlp = new MultiLayerPerceptron(16, 4, 2, new int[] { 42, 7 }, 0.1, activationFunction, dActivationFunction);

            double totalError = 0;
            do
            {
                totalError = 0;
                double error;
                mlp.Tranning(new double[] { 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 1, 0, 0, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 2, 1, 0, 0 }, new double[] { 0, 1, 0, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 2, 2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 0, 0, 1, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 2, 2 }, new double[] { 0, 0, 0, 1 }, out error);
                Console.WriteLine(error);
                totalError += error;
            }
            while (totalError/4 > 0.1);
            Console.ReadLine();
            do
            {
                totalError = 0;
                double error;
                mlp.Tranning(new double[] { 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 1, 0, 0, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 2, 1, 0, 0 }, new double[] { 0, 1, 0, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 2, 2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 0, 0, 1, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 2, 2 }, new double[] { 0, 0, 0, 1 }, out error);
                Console.WriteLine(error);
                totalError += error;
                mlp.Tranning(new double[] { 0, 0, 5, 6, 0, 0, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0 }, new double[] { 1, 0, 0, 0 }, out error);
                Console.WriteLine(error);
                totalError += error;
            }
            while (totalError / 5 > 0.1);
            double[] result = mlp.Compute(new double[] { 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0 });
            Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
            result = mlp.Compute(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 2, 1, 0, 0 });
            Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
            result = mlp.Compute(new double[] { 2, 2, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
            result = mlp.Compute(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 2, 2 });
            Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);

            result = mlp.Compute(new double[] { 0, 0, 5, 6, 0, 0, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0 });
            Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
            //result = mlp.Compute(new double[] { 6, 3, 2, 3 });
            //Console.WriteLine("{0} {1} {2} {3}", result[0], result[1], result[2], result[3]);
        }
    }
}
