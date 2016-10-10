using System;
using System.Collections.Generic;
using DecisionCriteria.Library;

namespace DecisionCriteria.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var matrix = new Matrix<int>();

            matrix.AddDecision("Decision 1", new List<int> {1, 2, 3, 4});
            matrix.AddDecision("Decision 2", new List<int> {11, -2, -3, -4});
            matrix.AddDecision("Decision 3", new List<int> {5, 12, 53, -14});
            matrix.AddDecision("Decision 4", new List<int> {90, -22, 3, 0});
            matrix.AddDecision("Decision 5", new List<int> {9, 21, 5, 0});
            



            matrix.States = new List<string>() { "I", "II", "III", "IV" };

            Console.WriteLine();
            Console.WriteLine("Best by minimax is: '{0}'", matrix.MiniMax());
            Console.WriteLine("Best by maxmax is: '{0}'", matrix.MaxMax());

            var parameter = 0d;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) is: '{1}'", parameter,
                matrix.Hurwicz(parameter));
            parameter = 0.1;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) is: '{1}'", parameter,
                matrix.Hurwicz(parameter));
            parameter = 0.5;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) is: '{1}'", parameter,
                matrix.Hurwicz(parameter));
            parameter = 0.9;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) is: '{1}'", parameter,
                matrix.Hurwicz(parameter));
            parameter = 1;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) is: '{1}'", parameter,
                matrix.Hurwicz(parameter));

            Console.WriteLine("Best by Bayes criteria (equals probabilities) is: '{0}'", matrix.Bayes());
            var probabilities = new[] {0.1, 0.1, 0.3, 0.5};
            Console.WriteLine("Best by Bayes criteria (probabilities: {0}) is: '{1}'", string.Join(", ", probabilities),
                matrix.Bayes(probabilities));
            probabilities = new[] {0.2, 0.5, 0.1, 0.2};
            Console.WriteLine("Best by Bayes criteria (probabilities: {0}) is: '{1}'", string.Join(", ", probabilities),
                matrix.Bayes(probabilities));

            Console.WriteLine("Best by Savage criteria is: '{0}'", matrix.Savage());
            Console.ReadKey();
        }
    }
}
