using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Names;

internal static class HistogramTask
{
    public static HistogramData GetBirthsPerDayHistogram(NameData[] names, string name)
    {
        var yLabel = new double[31];
        // var xLabel = new[] {
        //     "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", 
        //     "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
        //     "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        //     "31"
        // };
        var xLabel = new string[31];

        for (int i = 0; i < 31; i++)
            xLabel[i] = (i + 1).ToString();

        foreach (var item in names)
        {
            int day = item.BirthDate.Day;
            if (day != 1 && item.Name == name)
                yLabel[day - 1] += 1;
        }

        return new HistogramData(name, xLabel, yLabel);
    }
}