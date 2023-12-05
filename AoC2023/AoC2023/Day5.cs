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

    public static void TestA()
    {
        var day5 = new Day5();
        var solveA = day5.SolveA(TestData);
        Console.WriteLine(solveA);
    }

    private long SolveA(string input)
    {
        var lines = input.Split("\n");
        var ints = ConstructSeedNums(lines);
        var mapContent = ConstructMapContent(lines);

        long[] currentVals = ints;
        foreach (var map in mapContent)
        {
            currentVals = currentVals.Select(
                val =>
                {
                    var first = map.FirstOrDefault(mappedTo =>
                        mappedTo[1] <= val && val <= mappedTo[1] + mappedTo[2] - 1);
                    if (first == null)
                    {
                        return val;
                    }

                    return first[0] + val - first[1];
                }).ToArray();
        }

        return currentVals.Min();
    }

    private long[] ConstructSeedNums(string[] lines)
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

    public string SolveBFromFile(string filePath)
    {
        return "None";
    }
}