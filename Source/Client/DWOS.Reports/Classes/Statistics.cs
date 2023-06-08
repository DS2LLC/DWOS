using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace DWOS.Reports
{
    public class StatInfo
    {
        public double Median { get; set; }

        public double Mode { get; set; }

        public double Min { get; set; }

        public double Max { get; set; }

        //Central Tendency
        public double Mean { get; set; }

        //Dispersion
        public double Variance { get; set; }

        public double StdDeviation { get; set; }

        public string Units { get; set; }

        public override string ToString()
        {
            return Min + Units + " to " + Max + Units;
        }

        public static StatInfo Create(List<double> numbers, string units)
        {
            var stats = new DescriptiveStatistics(numbers);
            var modeList = numbers.GroupBy(values => Math.Round(values, 2)).Select(groups => new { Value = groups.Key, Occurrence = groups.Count() }).ToList();
            var maxOccurrence = modeList.Count > 0 ? modeList.Max(m => m.Occurrence) : 0;
            var mode = modeList.Where(m => m.Occurrence == maxOccurrence && maxOccurrence > 1).Select(x => x.Value).FirstOrDefault();

            return new StatInfo() { Min = Math.Round(stats.Minimum, 2), Max = Math.Round(stats.Maximum, 2), Mean = Math.Round(stats.Mean, 2), Median = Math.Round(stats.Mean, 2), Mode = mode, StdDeviation = Math.Round(stats.StandardDeviation, 2), Variance = Math.Round(stats.Variance, 2), Units = units };
        }
    }
}
