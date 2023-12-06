using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public class Day6 : DaySolution
{
    public string Day() => "Day 6";

    private const string TestInput = @"Time:      7  15   30
Distance:  9  40  200";

    public static void TestA()
    {
        var solve = SolveA(TestInput);
        Console.WriteLine(solve);

        Debug.Assert(solve == 288);
    }

    private static long SolveA(string input)
    {
        var lines = input.Split("\n");
        var times = GetAllNumbers(lines[0]);
        var distances = GetAllNumbers(lines[1]);

        return times.Zip(distances).Aggregate(1L, (aggr, tuple) =>
        {
            var (time, distance) = tuple;
            var isEven = time % 2 == 0;
            var maxX = isEven ? time / 2 : time / 2 + 1;
            var possibleSolutions = InfiniteRange(maxX, x => x - 1)
                .TakeWhile(x => distance < x * (time - x))
                .Count() * 2;
            possibleSolutions -= isEven ? 1 : 2;
            return aggr * possibleSolutions;
        });

        static long[] GetAllNumbers(string line)
        {
            return Regex.Matches(line, @"(\d+)")
                .Select(x => x.Groups.Values.First())
                .Select(x => x.Value)
                .Select(long.Parse)
                .ToArray();
        }
    }

    private static IEnumerable<long> InfiniteRange(long start, Func<long, long> step)
    {
        while (true)
        {
            yield return start;
            start = step(start);
        }
    }


    public static void TestB()
    {
        var solve = SolveB(TestInput);
        Console.WriteLine(solve);
        Debug.Assert(solve == 71503);
    }

    private static long SolveB(string input)
    {
        var lines = input.Split("\n");
        var time = GetNumber(lines[0]);
        var distance = GetNumber(lines[1]);


        var isEven = time % 2 == 0;
        var maxX = isEven ? time / 2 : time / 2 + 1;
        var possibleSolutions = InfiniteRange(maxX, x => x - 1)
            .TakeWhile(x => distance < x * (time - x))
            .Count() * 2;
        possibleSolutions -= isEven ? 1 : 2;
        return possibleSolutions;


        static long GetNumber(string line)
        {
            return long.Parse(
                Regex.Matches(line, @"(\d+)")
                    .Select(x => x.Groups.Values.First())
                    .Select(x => x.Value)
                    .Aggregate(new StringBuilder(), (sb, val) => sb.Append(val))
                    .ToString());
        }
    }

    public string SolveAFromFile(string filePath)
    {
        var streamReader = new StreamReader(filePath);
        return SolveA(streamReader.ReadToEnd()).ToString();
    }

    public string SolveBFromFile(string filePath)
    {
        var streamReader = new StreamReader(filePath);
        return SolveB(streamReader.ReadToEnd()).ToString();
    }
}