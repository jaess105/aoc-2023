// See https://aka.ms/new-console-template for more information

using AoC2023;

const string day3Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day3_input.txt";
const string day4Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day4_input.txt";
const string day5Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day5_input.txt";
const string day6Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day6_input.txt";
const string day7Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day7_input.txt";
const string day8Path = @"/home/jannik/proj/prog/aoc/aoc-2023/AoC2023/AoC2023/resources/day8_input.txt";



var arr = new (DaySolution, string)[]
{
    // (new Day3(), day3Path),
    // (new Day4(), day4Path),
    // (new Day5(), day5Path),
    // (new Day6(), day6Path),
    // (new Day7(), day7Path),
    (new Day8(), day8Path),
};

foreach (var (solution, path) in arr)
{
    Console.WriteLine($"{solution.Day()} a: {solution.SolveAFromFile(path)}");
    Console.WriteLine($"{solution.Day()} b: {solution.SolveBFromFile(path)}");
    Console.WriteLine();
}