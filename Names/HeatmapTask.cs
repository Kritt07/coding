using System.Runtime.CompilerServices;

namespace Names;

internal static class HeatmapTask
{
    public static HeatmapData GetBirthsPerDateHeatmap(NameData[] names)
    {
        var values = new double[30, 12];
        var days = new string[30];

        for (int i = 0; i < 30; i++)
            days[i] = (i + 2).ToString();

        var months = new string[12];

        for (int i = 0; i < 12; i++)
            months[i] = (i + 1).ToString();


        foreach (var name in names)
        {
            int day = name.BirthDate.Day;
            int month = name.BirthDate.Month;
            if (day != 1)
            {
                values[day - 2, month - 1] += 1;
            }
        }

        return new HeatmapData( "Карта интенсивностей", values, days, months);
        //return new HeatmapData(
        //    "Пример карты интенсивностей",
        //    new double[,] { { 1, 2, 3 }, { 2, 3, 4 }, { 3, 4, 4 }, { 4, 4, 4 } },
        //    new[] { "a", "b", "c", "d" },
        //    new[] { "X", "Y", "Z" });
    }
}