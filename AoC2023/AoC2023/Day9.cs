using System.Diagnostics;

namespace AoC2023;

public class Day9 : DaySolution
{
    public string Day() => "Day 9";

    private const string TestInput = @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";

    public static void TestA()
    {
        var solve = SolveA(TestInput);
        Console.WriteLine(solve);

        Debug.Assert(solve == 114);
    }

    public static void TestB()
    {
        var solve = SolveB(TestInput);
        Console.WriteLine(solve);

        Debug.Assert(solve == 2);
    }

    private static long SolveA(string input) => Solve(input, PredictionDirection.FuturePredict);
    private static long SolveB(string input) => Solve(input, PredictionDirection.PastPredict);

    private static int Solve(string input, PredictionDirection predictionDirection) =>
        ParseInput(input)
            .Select(
                sequence => ExtrapolateOneValue(sequence, predictionDirection)
            )
            .Sum();

    private static int ExtrapolateOneValue(
        List<int> sequence,
        PredictionDirection predictionDirection)
    {
        var allSequences = new List<List<int>> { sequence };
        var currentSequence = sequence;
        do
        {
            currentSequence = currentSequence.Zip(currentSequence.Skip(1))
                .Select((tuple) => tuple.Second - tuple.First)
                .ToList();
            allSequences.Add(currentSequence);
        } while (currentSequence.Any(x => x != 0));

        // Reverse the sequence for past prediction so that the same algorithm can be applied and values do not have
        // to be inserted in the beginning.
        if (predictionDirection is PredictionDirection.PastPredict)
        {
            allSequences.ForEach(x => x.Reverse());
        }

        Func<int, int, int> combinationFunc = predictionDirection switch
        {
            PredictionDirection.FuturePredict => (x, y) => x + y,
            PredictionDirection.PastPredict => (x, y) => x - y,
            _ => throw new ArgumentOutOfRangeException(nameof(predictionDirection), predictionDirection, null)
        };

        allSequences.Last().Add(0);
        for (var i = allSequences.Count - 1; i > 0; i--)
        {
            var seqBeforeLast = allSequences[i - 1];
            seqBeforeLast.Add(
                combinationFunc(seqBeforeLast.Last(), allSequences[i].Last())
            );
        }

        return allSequences.First().Last();
    }

    private static List<int>[] ParseInput(string input) =>
        input.Split("\n")
            .Select(
                s => s.Split(" ").Select(int.Parse).ToList()
            )
            .ToArray();


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

    private enum PredictionDirection
    {
        FuturePredict,
        PastPredict,
    }
}