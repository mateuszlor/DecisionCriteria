using System;
using System.Windows.Forms;
using DecisionCriteria.Library;
using ExcelReader;

namespace DecisionCriteria.ExcelReader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            #region Prepare

            var filename = OpenFile();

            Console.WriteLine("Selected Excel file with decision matrix: {0}", filename);

            var parser = new CsvParser
            {
                Filename = filename
            };

            var matrix = parser.ToMatrix();

            #endregion // Prepare

            Console.WriteLine(matrix.PrintAll());

            #region Calculate

            Console.WriteLine();
            Console.WriteLine("Best by minimax are / is {0}", string.Join(", ", matrix.MiniMaxMultiple()));
            Console.WriteLine("Best by maxmax are / is {0}", string.Join(", ", matrix.MaxMaxMultiple()));

            var parameter = 0d;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) are / is {1}",
                parameter, string.Join(", ", matrix.HurwiczMultiple(parameter)));
            parameter = 0.1;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) are / is {1}",
                parameter, string.Join(", ", matrix.HurwiczMultiple(parameter)));
            parameter = 0.5;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) are / is {1}",
                parameter, string.Join(", ", matrix.HurwiczMultiple(parameter)));
            parameter = 0.9;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) are / is {1}",
                parameter, string.Join(", ", matrix.HurwiczMultiple(parameter)));
            parameter = 1;
            Console.WriteLine("Best by Hurwicz criteria (parameter = {0}) are / is {1}",
                parameter, string.Join(", ", matrix.HurwiczMultiple(parameter)));

            Console.WriteLine("Best by Bayes criteria (probabilities not set) are '{0}",
                string.Join(", ", matrix.BayesMultiple()));

            var probabilities = new[] { 0.1, 0.1, 0.3, 0.5 };
            Console.WriteLine("Best by Bayes criteria (probabilities: {0}) are / is {1}",
                string.Join(", ", probabilities), string.Join(", ", matrix.BayesMultiple(probabilities)));
            probabilities = new[] { 0.2, 0.5, 0.1, 0.2 };
            Console.WriteLine("Best by Bayes criteria (probabilities: {0}) are / is {1}",
                string.Join(", ", probabilities), string.Join(", ", matrix.BayesMultiple(probabilities)));

            Console.WriteLine("Best by Savage criteria is: '{0}", string.Join(", ", matrix.SavageMultiple()));

            #endregion // Calculate

            Console.ReadKey();
        }

        public static string OpenFile()
        {
            var open = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv|All files|*.*" };

            var isOpened = open.ShowDialog();

            return open.FileName;
        }
    }
}
