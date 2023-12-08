using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Xsl;

namespace AoC2023;

public class Day8 : DaySolution
{
    public string Day() => "Day 8";

    private const string TestInput1 = @"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)";

    private const string TestInput2 = @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)";

    private const string TestInputB = @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)";

    public static void TestA1()
    {
        var solve = SolveA(TestInput1);
        Console.WriteLine(solve);

        Debug.Assert(solve == 2);
    }

    public static void TestA2()
    {
        var solve = SolveA(TestInput2);
        Console.WriteLine(solve);

        Debug.Assert(solve == 6);
    }

    public static void TestB()
    {
        var solve = SolveB(TestInputB);
        Console.WriteLine(solve);

        Debug.Assert(solve == 6);
    }

    private static long SolveA(string input)
    {
        var (map, instructions) = ToMapAndInstructions(input);

        var currentNode = "AAA";
        var step = 0;
        var instructionsLength = instructions.Length;
        while (currentNode != "ZZZ")
        {
            var (left, right) = map[currentNode];
            var instruction = instructions[step % instructionsLength];
            currentNode = instruction switch
            {
                'L' => left,
                'R' => right,
                _ => throw new ArgumentException("Unsupported instruction encountered")
            };

            step++;
        }

        return step;
    }

    private static long SolveB(string input)
    {
        var (map, instructions) = ToMapAndInstructions(input);

        var stopNodes = map.Keys.Where(k => k.EndsWith("Z")).ToHashSet();
        IEnumerable<string> currentNodes = map.Keys.Where(k => k.EndsWith("A")).ToArray();

        var step = 0;
        var instructionsLength = instructions.Length;
        var stepsToEnds = new List<long>();
        while (currentNodes.Any(cn => !stopNodes.Contains(cn)))
        {
            var nextNodes = new HashSet<string>();
            var index = step % instructionsLength;

            foreach (var currentNode in currentNodes)
            {
                var (left, right) = map[currentNode];
                var instruction = instructions[index];

                var nextNode = instruction switch
                {
                    'L' => left,
                    'R' => right,
                    _ => throw new ArgumentException("Unsupported instruction encountered")
                };

                if (!stopNodes.Contains(nextNode))
                {
                    nextNodes.Add(nextNode);
                    continue;
                }

                stepsToEnds.Add(step + 1);
            }

            currentNodes = nextNodes;
            step++;
        }


        return Lcm(stepsToEnds);
    }

    private static long Lcm(IEnumerable<long> numbers) => numbers.Aggregate(Lcm);

    private static long Lcm(long a, long b)
    {
        return (a / Gcf(a, b)) * b;
    }

    private static long Gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }


    private static (Dictionary<string, (string, string)>, char[]) ToMapAndInstructions(string input)
    {
        var lines = input.Split("\n");
        var instructions = lines[0].ToCharArray();
        var map = lines.Skip(2)
            .Select(s => Regex.Match(s, @"(\w{3}) = \((\w{3}), (\w{3})\)"))
            .Select(m => (m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value))
            .ToDictionary(
                tuple => tuple.Item1,
                tuple => (tuple.Item2, tuple.Item3)
            );
        return (map, instructions);
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