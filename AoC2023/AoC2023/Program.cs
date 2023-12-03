// See https://aka.ms/new-console-template for more information

using AoC2023;

const string day3Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day3_input.txt";

var arr = new DaySolution[]
{
    new Day3(),
};

foreach (var solution in arr)
{
    Console.WriteLine($"{solution.Day()} a: {solution.SolveAFromFile(day3Path)}");
    Console.WriteLine($"{solution.Day()} b: {solution.SolveBFromFile(day3Path)}");
    Console.WriteLine();
}