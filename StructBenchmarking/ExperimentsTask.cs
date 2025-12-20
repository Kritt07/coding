using System;
using System.Collections.Generic;

namespace StructBenchmarking;

public class Experiments
{
    public static ChartData BuildChartDataForArrayCreation(
        IBenchmark benchmark, int repetitionsCount)
    {
        return BuildChartData(
            benchmark,
            repetitionsCount,
            "Create array",
            size => new ClassArrayCreationTask(size),
            size => new StructArrayCreationTask(size)
        );
    }

    public static ChartData BuildChartDataForMethodCall(
        IBenchmark benchmark, int repetitionsCount)
    {
        return BuildChartData(
            benchmark,
            repetitionsCount,
            "Call method with argument",
            size => new MethodCallWithClassArgumentTask(size),
            size => new MethodCallWithStructArgumentTask(size)
        );
    }

    private static ChartData BuildChartData(
        IBenchmark benchmark,
        int repetitionsCount,
        string title,
        Func<int, ITask> createClassTask,
        Func<int, ITask> createStructTask)
    {
        var classesTimes = new List<ExperimentResult>();
        var structuresTimes = new List<ExperimentResult>();

        foreach (var size in Constants.FieldCounts)
        {
            // Измеряем время для класса
            var classTask = createClassTask(size);
            double classTime = benchmark.MeasureDurationInMs(classTask, repetitionsCount);
            classesTimes.Add(new ExperimentResult(size, classTime));

            // Измеряем время для структуры
            var structTask = createStructTask(size);
            double structTime = benchmark.MeasureDurationInMs(structTask, repetitionsCount);
            structuresTimes.Add(new ExperimentResult(size, structTime));
        }

        return new ChartData
        {
            Title = title,
            ClassPoints = classesTimes,
            StructPoints = structuresTimes,
        };
    }
}