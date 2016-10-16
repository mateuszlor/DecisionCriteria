using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionCriteria.Library
{
    /// <summary>
    /// Decision matrix
    /// Operates on table of utility
    /// Per-state value is double
    /// </summary>
    public class Matrix : Matrix<double>
    {

    }

    /// <summary>
    /// Decision matrix
    /// Operates on table of utility
    /// </summary>
    /// <typeparam name="T">Value type used in comapration. If T is not simple numeric type you must provide selector</typeparam>
    public class Matrix<T>// where T : IComparable
    {
        #region Fields

        private Dictionary<string, List<T>> _decisions;
        private Func<T, double> _selector;
        private bool _isSimpleNumeric;
        private IList<double> _probabilities;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Selector used to determine numeric value of generic type T if T is not simple comparable type
        /// </summary>
        public Func<T, double> Selector
        {
            get { return _selector; }
            set
            {
                _selector = value;
                ValidateType();
            }
        }

        /// <summary>
        /// Returns if inner decision matrix is valid (each decision covers all states)
        /// </summary>
        public bool IsValid
        {
            get
            {
                var valid = _decisions.Values.Max(x => x.Count) == _decisions.Values.Min(x => x.Count);
                return States == null ? valid : valid && States.Count == _decisions.First().Value.Count;
            }
        }

        /// <summary>
        /// State names
        /// </summary>
        public IList<string> States { get; set; }

        public IList<double> Probabilities
        {
            get
            {
                return _probabilities;
                }
            set
            {
                if (!value.Sum().Equals(1))
                {
                    throw new ArgumentException("Probabilities must sum up to 1", nameof(Probabilities));
                }
                _probabilities = value;
            }
        }

        /// <summary>
        /// Dissapointment matrix
        /// </summary>
        public Dictionary<string, List<T>> Dissapointment
        {
            get
            {
                var result = new Dictionary<string, List<T>>();

                foreach (var decision in _decisions)
                {
                    var max = decision.Value.Max();
                    var values = decision.Value
                        .Select(value =>
                            _isSimpleNumeric
                                ? (dynamic) max - (dynamic) value
                                : (dynamic) Selector(max) - (dynamic) Selector(value))
                        .Select(x => (T) x)
                        .ToList();

                    result.Add(decision.Key, values);
                }
                return result;
            }
        }

        /// <summary>
        /// Loss matrix
        /// </summary>
        public Dictionary<string, List<T>> Loss
        {
            get
            {
                var maxColumn = new List<T>();

                for (var i = 0; i < _decisions.Values.First().Count; i++)
                {
                    maxColumn.Add(_decisions.Values.Select(x => x[i]).Max());
                }

                var result = new Dictionary<string, List<T>>();

                foreach (var decision in _decisions)
                {
                    var values = decision.Value
                        .Select((value, i) =>
                            _isSimpleNumeric
                                ? (dynamic) maxColumn[i] - (dynamic) value
                                : (dynamic) Selector(maxColumn[i]) - (dynamic) Selector(value))
                        .Select(x => (T) x)
                        .ToList();

                    result.Add(decision.Key, values);
                }
                return result;
            }
        }

        #endregion // Properties

        #region Methods

        #region Helpers
        private void ValidateType()
        {
            var x1 = default(T);
            var x2 = default(T);

            try
            {
                var r = (dynamic)x1 - (dynamic)x2;
                r.Equals(default(T));

                _isSimpleNumeric = true;
                _selector = x => x as dynamic;
            }
            catch (Exception e1)
            {
                try
                {
                    var r = (dynamic)Selector(x1) - (dynamic)Selector(x2);
                    r.Equals((dynamic) Selector(default(T)));

                    _isSimpleNumeric = false;
                }
                catch (Exception e2)
                {
                    var message = string.Format("Generic type {0} is not valid numeric type", typeof(T));
                    throw new ArgumentException(message, nameof(T), new AggregateException(e1, e2));
                }
            }
        }

        public void AddDecision(string decisionName, List<T> utilityValues)
        {
            _decisions.Add(decisionName, utilityValues);
        }

        public void AddDecision(KeyValuePair<string, List<T>> decision)
        {
            AddDecision(decision.Key, decision.Value);
        }

        public void AddDecisionRange(Dictionary<string, List<T>> decisions)
        {
            foreach (var decision in decisions)
            {
                AddDecision(decision);
            }
        }

        private string Print(Dictionary<string, List<T>> matrix)
        {
            var sb = new StringBuilder();
            var stateCount = matrix.Max(x => x.Value.Count);

            // header
            var headers = new List<string>();

            if (States == null)
            {
                for (var i = 0; i < stateCount; i++)
                {
                    headers.Add(string.Format("state_{0}", i));
                }
            }
            else
            {
                headers.AddRange(States);
            }

            var tabCount = Math.Ceiling((matrix.Keys.Max(x => x.Length) + 1) / 8d);
            var tabs = string.Empty;

            for (var i = 0; i < tabCount; i++)
            {
                tabs += "\t";
            }

            sb.AppendLine(tabs + string.Join("\t", headers));

            // decisions
            foreach (var decision in matrix)
            {
                var noTabCount = Math.Ceiling((decision.Key.Length + 1) / 8d);

                var currentTabsCount = tabCount - noTabCount + 1;

                tabs = string.Empty;

                for (var i = 0; i < currentTabsCount; i++)
                {
                    tabs += "\t";
                }

                sb.AppendLine(decision.Key + tabs + string.Join("\t", decision.Value));
            }

            return sb.ToString();
        }

        public string Print()
        {
            return Print(_decisions);
        }

        public string PrintLoss()
        {
            return Print(Loss);
        }
        public string PrintDissapointment()
        {
            return Print(Dissapointment);
        }

        #endregion // Helpers

        #region Decisions

        private Dictionary<string, double> CalculateMin(Dictionary<string, List<T>> matrix)
        {
            return matrix.Select(x => new
            {
                x.Key,
                Min = x.Value.Select(Selector).Min()
            }).ToDictionary(x => x.Key, x => x.Min);
        }

        private Dictionary<string, double> CalculateMax(Dictionary<string, List<T>> matrix)
        {
            return matrix.Select(x => new
            {
                x.Key,
                Min = x.Value.Select(Selector).Max()
            }).ToDictionary(x => x.Key, x => x.Min);
        }

        public string MiniMax()
        {
            return CalculateMin(_decisions).OrderByDescending(x => x.Value).First().Key;
        }

        public List<string> MiniMaxMultiple()
        {
            var minDictionary = CalculateMin(_decisions);
            var max = minDictionary.OrderByDescending(x => x.Value).First();
            return minDictionary.Where(x => x.Value.Equals(max.Value)).Select(x => x.Key).ToList();
        }

        public string MaxMax()
        {
            return CalculateMax(_decisions).OrderByDescending(x => x.Value).First().Key;
        }

        public List<string> MaxMaxMultiple()
        {
            var maxDictionary = CalculateMax(_decisions);
            var max = maxDictionary.OrderByDescending(x => x.Value).First();
            return maxDictionary.Where(x => x.Value.Equals(max.Value)).Select(x => x.Key).ToList();
        }

        public List<string> HurwiczMultiple(double realismFactor)
        {
            if (realismFactor < 0 || realismFactor > 1)
            {
                throw new ArgumentException("Realism factor should not be < 0 and > 1", nameof(realismFactor));
            }
            var number = (double)Convert.ChangeType(default(T), typeof(double));
            if (!number.Equals(0))
            {
                throw new InvalidOperationException("Hurwicz criterion can be used only for numeric values");
            }
            var result = _decisions.Select(d =>
                new
                {
                    d,
                    resultValue = realismFactor * d.Value.Select(Selector).Min() +
                                   (1 - realismFactor) * d.Value.Select(Selector).Max()
                });
            var bestValue = result.OrderByDescending(x => x.resultValue).First().resultValue;

            return result.Where(x => x.resultValue.Equals(bestValue)).Select(x => x.d.Key).ToList();
        }

        public string Hurwicz(double realismFactor)
        {
            return HurwiczMultiple(realismFactor).First();
        }

        private Dictionary<string, double> CalculateBayes(IList<double> probabilities)
        {
            var number = (double)Convert.ChangeType(default(T), typeof(double));
            if (!number.Equals(0))
            {
                throw new InvalidOperationException("Bayes criterion can be used only for numeric values");
            }

            var calculated = new Dictionary<string, double>();

            foreach (var decision in _decisions)
            {
                var singleResult = decision.Value.Select((t, i) => probabilities[i]*Selector(t)).Sum();
                calculated.Add(decision.Key, singleResult);
            }
            return calculated;
        }

        public string Bayes()
        {
            IList<double> probabilities;

            if (_probabilities == null || !_probabilities.Any())
            {
                var statesCount = _decisions.First().Value.Count;
                probabilities = Enumerable.Repeat(1d/statesCount, statesCount).ToList();
            }
            else
            {
                probabilities = _probabilities;
            }

            return Bayes(probabilities);
        }

        public List<string> BayesMultiple()
        {
            IList<double> probabilities;

            if (_probabilities == null || !_probabilities.Any())
            {
                var statesCount = _decisions.First().Value.Count;
                probabilities = Enumerable.Repeat(1d / statesCount, statesCount).ToList();
            }
            else
            {
                probabilities = _probabilities;
            }

            return BayesMultiple(probabilities);
        }

        public string Bayes(IList<double> probabilities)
        {
            if (!probabilities.Sum().Equals(1))
            {
                throw new ArgumentException("Probabilities must sum up to 1", nameof(probabilities));
            }
            return CalculateBayes(probabilities).OrderByDescending(x => x.Value).First().Key;
        }

        public List<string> BayesMultiple(IList<double> probabilities)
        {
            var calculated = CalculateBayes(probabilities);
            var best = calculated.OrderByDescending(x => x.Value).First().Value;
            return calculated.Where(x => x.Value.Equals(best)).Select(x => x.Key).ToList();
        }

        public string Savage()
        {
            return CalculateMax(Loss).OrderByDescending(x => x.Value).First().Key;
        }

        public List<string> SavageMultiple()
        {
            var maxDictionary = CalculateMax(_decisions);
            var max = maxDictionary.OrderByDescending(x => x.Value).First();
            return maxDictionary.Where(x => x.Value.Equals(max.Value)).Select(x => x.Key).ToList();
        }

        #endregion // Decisions

        #endregion // Methods

        #region Constructor

        /// <summary>
        /// Creates new generic matrix.
        /// Use this constructor if T is comparable
        /// </summary>
        public Matrix()
        {
            _decisions = new Dictionary<string, List<T>>();
            ValidateType();
        }

        /// <summary>
        /// Creates new generic matrix.
        /// Use this constructor in order to operate on complex non comparable types
        /// </summary>
        /// <param name="selector">Numeric value selector</param>
        public Matrix(Func<T, double> selector)
        {
            Selector = selector;
            _decisions = new Dictionary<string, List<T>>();
            ValidateType();
        }

        #endregion // Constructor
    }
}
