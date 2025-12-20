using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace StructBenchmarking;

public class Benchmark : IBenchmark
{
    public double MeasureDurationInMs(ITask task, int repetitionCount)
    {
        // Принудительный вызов GC для минимизации его влияния на измерения
        GC.Collect();
        GC.WaitForPendingFinalizers();

        task.Run();

        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < repetitionCount; i++)
        {
            task.Run();
        }

        stopwatch.Stop();
        return (double)stopwatch.ElapsedMilliseconds / repetitionCount;
    }
}

public class StringBuilderTask : ITask
{
    public void Run()
    {
        var builder = new StringBuilder();
        for (int i = 0; i < 10000; i++)
        {
            builder.Append('a');
        }
        string result = builder.ToString();
    }
}

public class StringConstructorTask : ITask
{
    public void Run()
    {
        string result = new string('a', 10000);
    }
}

[TestFixture]
public class RealBenchmarkUsageSample
{
    [Test]
    public void StringConstructorFasterThanStringBuilder()
    {
        var benchmark = new Benchmark();

        var stringBuilderTask = new StringBuilderTask();
        var stringConstructorTask = new StringConstructorTask();

        int repetitionCount = 10000;
        double stringBuilderTime = benchmark.MeasureDurationInMs(stringBuilderTask, repetitionCount);
        double stringConstructorTime = benchmark.MeasureDurationInMs(stringConstructorTask, repetitionCount);

        ClassicAssert.Less(stringConstructorTime, stringBuilderTime,
            $"String constructor time: {stringConstructorTime} ms, " +
            $"StringBuilder time: {stringBuilderTime} ms");
    }
}