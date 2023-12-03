using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC2023;

public class Day3 : DaySolution
{
    private const string TestInput = @"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..";

    public static void TestA()
    {
        var solve = SolveA(TestInput);
        Debug.Assert(solve == 4361);
        Console.WriteLine(solve);
    }

    public static void TestB()
    {
        var solve = SolveB(TestInput);
        Debug.Assert(solve == 467835);
        Console.WriteLine(solve);
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

    public string Day()
    {
        return "Day3";
    }

    private static int SolveB(string input)
    {
        var matrix = ToMatrix(input);

        var symbols = GetSymbols(matrix);

        var stars = symbols.Where(s => s.Item1 == '*');
        var adjacentNumbers = FindAdjacentNumbersBySymbol(stars, matrix);

        return adjacentNumbers
            .Where(numbers => numbers.Count == 2)
            .Select(numbers => numbers[0] * numbers[1])
            .Sum();
    }

    private static int SolveA(string input)
    {
        var matrix = ToMatrix(input);

        var symbols = GetSymbols(matrix);

        var adjacentNumbers = FindAdjacentNumbers(symbols, matrix);

        return adjacentNumbers.Sum();
    }


    private static char[][] ToMatrix(string input)
    {
        return input.Split("\n").Select(x => x.ToCharArray().ToArray()).ToArray();
    }

    private static List<int> FindAdjacentNumbers(
        IEnumerable<(char, int, int)> symbols,
        char[][] matrix)
    {
        var numbers = new List<int>();
        var foundIndices = new HashSet<(int, int)>();

        foreach (var (_, i, j) in symbols)
        {
            numbers.AddRange(FindAdjacentNumbers(i, j, matrix, foundIndices));
        }

        return numbers;
    }

    private static List<List<int>> FindAdjacentNumbersBySymbol(
        IEnumerable<(char, int, int)> symbols,
        char[][] matrix)
    {
        var numbersBySymbol = new List<List<int>>();

        foreach (var (symbol, i, j) in symbols)
        {
            var foundIndices = new HashSet<(int, int)>();
            numbersBySymbol.Add(FindAdjacentNumbers(i, j, matrix, foundIndices));
        }

        return numbersBySymbol;
    }

    private static List<int> FindAdjacentNumbers(
        int i, int j, char[][] matrix, HashSet<(int, int)> foundIndices)
    {
        (int, int)[] neighbouringIndices = new (int, int)[]
        {
            (i + 1, j + 1),
            (i + 1, j),
            (i, j + 1),
            (i - 1, j - 1),
            (i - 1, j),
            (i, j - 1),
            (i - 1, j + 1),
            (i + 1, j - 1)
        };
        var numbers = neighbouringIndices
            .Select((index) => (matrix[index.Item1][index.Item2], index.Item1, index.Item2))
            .Where(triple => char.IsDigit(triple.Item1))
            .Select(triple => ((int)(triple.Item1 - '0'), triple.Item2, triple.Item3));

        var adjacentNumbers = new List<int>();
        foreach (var (c, x, y) in numbers)
        {
            if (foundIndices.Contains((x, y)))
                continue;
            var (number, numsIndices) = FindWholeNumber(x, y, matrix);
            if (foundIndices.Overlaps(numsIndices))
                continue;
            foundIndices.UnionWith(numsIndices);
            adjacentNumbers.Add(number);
        }

        return adjacentNumbers;
    }

    private static (int, HashSet<(int, int)>) FindWholeNumber(int i, int j, char[][] matrix)
    {
        HashSet<(int, int)> indices = new() { (i, j) };
        StringBuilder number = new();
        int length = matrix[0].Length;

        int backwardJ = j;
        while (backwardJ > 0 && char.IsDigit(matrix[i][backwardJ - 1]))
        {
            backwardJ--;
            number.Insert(0, matrix[i][backwardJ]);
            indices.Add((i, backwardJ));
        }

        number.Append(matrix[i][j]);

        int forwardJ = j;
        while (length > forwardJ + 1 && char.IsDigit(matrix[i][forwardJ + 1]))
        {
            forwardJ++;
            number.Append(matrix[i][forwardJ]);
            indices.Add((i, forwardJ));
        }


        return (int.Parse(number.ToString()), indices);
    }


    private static IEnumerable<(char, int, int)> GetSymbols(char[][] matrix)
    {
        return matrix.SelectMany((line, i) =>
            line.Select((s, j) => (s, i, j))
                .Where((triple) => Regex.IsMatch(triple.s.ToString(), @"[^\d\.]")));
    }

    public static void TestFindWholeNumber()
    {
        var matrix = ToMatrix(TestInput);
        var x1 = FindWholeNumber(0, 0, matrix);
        var x2 = FindWholeNumber(0, 1, matrix);
        var x3 = FindWholeNumber(0, 2, matrix);

        Debug.Assert(x1.Item1 == x2.Item1 && x1.Item1 == x3.Item1 && x1.Item1 == 467);
    }
}

public static class StringArrayExtension
{
    public static String ToMap(this String[][] self)
    {
        return string.Join("\n", self.Select(sArr => string.Join(", ", sArr)));
    }
}