using System.Text.RegularExpressions;

namespace AoC2023;

public class Day4 : DaySolution
{
    public string Day() => "Day 4";

    private const string TestData = @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

    public static void TestA()
    {
        var day4 = new Day4();
        var solveA = day4.SolveA(TestData);
        Console.WriteLine(solveA);
    }

    public static void TestB()
    {
        var day4 = new Day4();
        var solveB = day4.SolveB(TestData);
        Console.WriteLine(solveB);
    }

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

    private int SolveB(string input)
    {
        var inputData = ConvertInput(input);
        var dataCopiesCount = inputData.Select((data, i) => (i, data))
            .ToDictionary(tuple => tuple.i, tuple => 1);

        foreach (var i in dataCopiesCount.Keys)
        {
            var data = inputData[i];
            var numOfWins = data.GottenNumbers.Count(x => data.WinningNumbers.Contains(x));

            foreach (var copyOf in Enumerable.Range(i + 1, numOfWins))
            {
                dataCopiesCount[copyOf] += dataCopiesCount[i];
            }
        }


        return dataCopiesCount.Values.Sum();
    }

    private int SolveA(string input)
    {
        var inputData = ConvertInput(input);

        var scoreMap = new Dictionary<int, int>();
        foreach (var data in inputData)
        {
            scoreMap[data.GroupNumber] = CalcScore(data);
        }

        return scoreMap.Values.Sum();
    }

    private static int CalcScore(InputData data)
    {
        var score = 0;
        foreach (var _ in data.GottenNumbers.Where(x => data.WinningNumbers.Contains(x)))
        {
            score = score == 0 ? 1 : score * 2;
        }

        return score;
    }

    private static InputData[] ConvertInput(string input)
    {
        var result = input.Split("\n")
            .Select(LineToInputData)
            .ToArray();

        return result;


        InputData LineToInputData(string line)
        {
            var match = Regex.Match(line, @"Card +(\d+):((?: *\d+)+) \|((?: *\d+)+)");
            var cardNr = int.Parse(match.Groups[1].Value);
            var gottenNumbs = MatchedGroupToInts(match.Groups[2].Value);
            var winningNumbers = MatchedGroupToInts(match.Groups[3].Value);

            return new InputData(cardNr, gottenNumbs, winningNumbers);
        }

        int[] MatchedGroupToInts(string group) =>
            Regex.Split(group, @"\s+")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Select(int.Parse)
                .ToArray();
    }
}

internal struct InputData
{
    public readonly int GroupNumber;
    public readonly int[] GottenNumbers;
    public readonly HashSet<int> WinningNumbers;

    public InputData(int groupNumber, int[] gottenNumbers, int[] winningNumbers)
    {
        GroupNumber = groupNumber;
        GottenNumbers = gottenNumbers;
        WinningNumbers = winningNumbers.ToHashSet();
    }
}