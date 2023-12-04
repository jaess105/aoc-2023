// See https://aka.ms/new-console-template for more information

using AoC2023;

const string day3Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day3_input.txt";
const string day4Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day4_input.txt";

var arr = new (DaySolution, string)[]
{
    (new Day3(), day3Path),
    (new Day4(), day4Path),
};

foreach (var (solution, path) in arr)
{
    Console.WriteLine($"{solution.Day()} a: {solution.SolveAFromFile(path)}");
    Console.WriteLine($"{solution.Day()} b: {solution.SolveBFromFile(path)}");
    Console.WriteLine();
}