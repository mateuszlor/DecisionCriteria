using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DecisionCriteria.Library;

namespace ExcelReader
{
    class CsvParser
    {
        public string Filename { get; set; }

        public Matrix ToMatrix()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                throw new ArgumentNullException(nameof(Filename), "You must provide filename");
            }
            
            var matrix = new Matrix();

            try
            {
                var csv = File.ReadAllLines(Filename);

                var header = csv[0].Split(';').Skip(1).ToList();

                var skip = 1;

                var p = csv[1].Split(';');

                if (p[0].ToLower() == "probabilities")
                {
                    skip = 2;
                    matrix.Probabilities = p.Skip(1).Select(double.Parse).ToList();
                }

                var data =
                    csv.Skip(skip)
                        .Select(l => l.Split(';'))
                        .Select(l =>
                            new KeyValuePair<string, List<double>>
                                (l[0], l
                                    .Skip(1)
                                    .Select(double.Parse)
                                    .ToList()))
                        .ToDictionary(x => x.Key, x => x.Value);

                matrix.AddDecisionRange(data);
                matrix.States = header;
            }
            catch (FileNotFoundException e)
            {
                throw new ArgumentException(string.Format("Not found provided file: {0}", Filename), nameof(Filename), e);
            }
            catch (FormatException e)
            {
                throw new ArgumentException("One or more numeric value in file is invalid", e);
            }

            return matrix;
        }
    }
}
