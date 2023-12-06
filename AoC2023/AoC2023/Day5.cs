using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public class Day5 : DaySolution
{
    public string Day() => "Day 5";

    private const string TestData = @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";

    public string SolveAFromFile(string filePath)
    {
        using var streamReader = new StreamReader(filePath);
        return SolveA(streamReader.ReadToEnd()).ToString();
    }

    public string SolveBFromFile(string filePath)
    {
        using var streamReader = new StreamReader(filePath);
        return SolveB(streamReader.ReadToEnd()).ToString();
    }

    public static void TestA()
    {
        var day5 = new Day5();
        var solveA = day5.SolveA(TestData);
        Console.WriteLine(solveA);
    }

    private long SolveA(string input)
    {
        var lines = input.Split("\n");
        var ints = ConstructSeedNumsA(lines);
        var mapContent = ConstructMapContent(lines);

        Func<long, long> seedToLocation = SeedToLocationFunc(mapContent);

        return ints.Select(seedToLocation).Min();
    }


    public static void TestB()
    {
        var day5 = new Day5();
        var solveB = day5.SolveB(TestData);
        Console.WriteLine(solveB);
    }

    private long SolveB(string input)
    {
        var lines = input.Split("\n");
        var ints = ConstructSeedNumsB(lines);
        var mapContent = ConstructMapContent(lines);

        Func<long, long> seedToLocation = SeedToLocationFunc(mapContent);
        ConcurrentBag<long> collector = new();
        var ranges = ints
            .EvenlyDistribute()
            .OrderBy(tuple => tuple.Item2)
            .ToArray();
        var threads = ranges.Select((startAndLength, i) =>
            new Thread(() => collector.Add(
                GetMinumalLocationFromSeed(GenerateIntsFromInput(
                        startAndLength.Item1,
                        startAndLength.Item2,
                        i
                    ),
                    seedToLocation
                )
            ))).ToArray();


        Console.Write($"Starting {threads.Length} threads!");
        Stopwatch stopwatch = new();
        stopwatch.Start();
        foreach (var thread in threads)
        {
            thread.Start();
        }

        LoopJoinThreads(threads);


        stopwatch.Stop();
        Console.WriteLine($"It took {stopwatch.ElapsedMilliseconds} ms to run.");
        return collector.Min();
    }

    private static void LoopJoinThreads(Thread[] threads)
    {
        var livingThread = true;
        while (livingThread)
        {
            Thread?[] finishedThreads = threads.Where(t => !t.IsAlive).ToArray();
            for (int i = 0; i < finishedThreads.Length; i++)
            {
                var finishedThread = finishedThreads[i];
                if (finishedThread is null || !finishedThread.Join(10))
                {
                    finishedThreads[i] = null;
                }
            }

            threads = threads.Except(finishedThreads.WithoutNull()).ToArray();
            livingThread = threads.Length > 0;
        }
    }


    private static long GetMinumalLocationFromSeed(IEnumerable<long> ints, Func<long, long> seedToLocation)
    {
        return ints.Select(seedToLocation).Min();
    }

    private static IEnumerable<long> GenerateIntsFromInput(long start, long length, int? threadNum = null)
    {
        var end = start + length;

        Console.WriteLine($"Started Sequence {start} until {end}!");

        for (long i = start; i < start + length; i++)
        {
            if (i % 10_000_000 == 0)
            {
                StringBuilder sb = new();
                sb.Append($"Completed ")
                    .Append(i)
                    .Append("/")
                    .Append(end)
                    .Append(", ")
                    .Append(end - i);
                if (threadNum is not null)
                {
                    sb.Append(" on Thread ").Append(threadNum);
                }

                sb.Append(" left!");
                Console.WriteLine(sb);
            }

            yield return i;
        }

        Console.WriteLine("Ended Sequence on Thread" + threadNum);
    }

    private (long, long)[] ConstructSeedNumsB(string[] lines)
    {
        var seedNums = lines[0].Split(":")[1]
            .Split(" ")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(long.Parse)
            .Aggregate(new List<List<long>>(),
                (aggr, x) =>
                {
                    if (aggr.Count == 0 || aggr.Last().Count == 2)
                    {
                        aggr.Add(new List<long> { x });
                    }
                    else
                    {
                        aggr.Last().Add(x);
                    }

                    return aggr;
                })
            .Select(l => (l[0], l[1]))
            .ToArray();

        return seedNums;
    }


    private static Func<long, long> SeedToLocationFunc(IEnumerable<IEnumerable<long[]>> mapContent)
    {
        return val =>
        {
            foreach (var map in mapContent)
            {
                var tmpVal = val;
                var first = map.FirstOrDefault(mappedTo =>
                    mappedTo[1] <= tmpVal && tmpVal <= mappedTo[1] + mappedTo[2] - 1);
                if (first == null)
                {
                    continue;
                }

                val = Math.Abs(first[0] + val - first[1]);
            }

            return val;
        };
    }

    private long[] ConstructSeedNumsA(string[] lines)
    {
        var seedNums = lines[0].Split(":")[1]
            .Split(" ")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(long.Parse)
            .ToArray();
        return seedNums;
    }

    private List<List<long[]>> ConstructMapContent(string[] lines)
    {
        int listCounter = -1;
        List<List<long[]>> mapInputs = new();

        foreach (var line in lines.Skip(2).Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            if (Regex.IsMatch(line, @".*map:"))
            {
                mapInputs.Add(new List<long[]>());
                listCounter++;
                continue;
            }

            mapInputs[listCounter].Add(
                line.Split(" ").Select(long.Parse).ToArray()
            );
        }

        return mapInputs;
    }
}

public static class LongLongArrExtensions
{
    public static (long, long)[] EvenlyDistribute(this (long, long)[] self)
    {
        var minLength = self.Select(x => x.Item1).Min();
        bool changed = true;
        var current = self;
        while (changed)
        {
            changed = false;
            current = current.SelectMany(tuple =>
            {
                if (tuple.Item2 <= minLength)
                {
                    return new[] { tuple };
                }

                changed = true;
                return Partition(tuple);
            }).ToArray();
        }

        return current;
    }

    public static (long, long)[] SplitTheLongest(this (long, long)[] self, int n)
    {
        if (n < 1)
        {
            throw new ArgumentException($"N cannot be smaller than 1 but was{n}");
        }

        var counter = 0;
        var arr = new (long, long)[self.Length + n];
        foreach (var tuple in self.OrderByDescending(x => x.Item2))
        {
            if (counter < n * 2)
            {
                var split = Partition(tuple);
                Debug.Assert(split[0].Item2 + split[1].Item2 == tuple.Item2);
                arr[counter++] = split[0];
                arr[counter++] = split[1];
            }
            else
            {
                arr[counter++] = tuple;
            }
        }

        return arr;
    }


    private static (long, long)[] Partition((long, long) tuple)
    {
        var (start, length) = tuple;
        var half = length / 2;
        return new[] { (start, half), (start + half, length - half) };
    }
}

public static class EnumerableExtension
{
    /// <summary>
    /// Filters out all null values from a nullable IEnumerable. Convenience method to remove compiler warnings while
    /// filtering out all nullable values.
    /// </summary>
    /// <remarks>When doing this with a LINQ Where clause the compile does not understand that the type is now non
    /// nullable and produces a warning. With this method the compiler does understand and the warning disappears.</remarks>
    /// <param name="self">The IEnumerable of a nullable type.</param>
    /// <typeparam name="T">The collections type.</typeparam>
    /// <returns>The same IEnumerable with all null values removed and a non nullable type.</returns>
    public static IEnumerable<T> WithoutNull<T>(this IEnumerable<T?> self)
    {
        foreach (var el in self)
        {
            if (el is not null)
            {
                yield return el!;
            }
        }
    }
}