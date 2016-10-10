using System.Text;

namespace DecisionCriteria.Library
{
    public static class ToStringUtilities
    {
        public static string PrintAll(this Matrix matrix)
        {
            var sb = new StringBuilder();

            sb.AppendLine(new string('=', 50));

            sb.AppendLine("MATRIX");
            sb.AppendLine(matrix.Print());

            sb.AppendLine(new string('=', 50));

            sb.AppendLine("LOSS");
            sb.AppendLine(matrix.PrintLoss());

            sb.AppendLine(new string('=', 50));

            sb.AppendLine("DISSAPOINTMENT");
            sb.AppendLine(matrix.PrintDissapointment());

            sb.AppendLine(new string('=', 50));

            return sb.ToString();
        }
    }
}
