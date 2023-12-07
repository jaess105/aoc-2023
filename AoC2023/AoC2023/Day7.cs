using System.Diagnostics;

namespace AoC2023;

public class Day7 : DaySolution
{
    public string Day() => "Day 7";

    private const string TestInput = @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483";

    public static void TestA()
    {
        var solve = SolveA(TestInput);
        Console.WriteLine(solve);

        Debug.Assert(solve == 6440);
    }

    public static void TestB()
    {
        var solve = SolveB(TestInput);
        Console.WriteLine(solve);

        Debug.Assert(solve == 5905);
    }


    private static long SolveA(string input) => Solve(input, Hand.ConstructHandForA);
    private static long SolveB(string input) => Solve(input, Hand.ConstructHandForB);

    private static long Solve(string inputContent, Func<(char[] cards, long bid), Hand> constructHand)
    {
        var input = inputContent.Split("\n")
            .Select(s => s.Split(" "))
            .Select(sArr => (sArr[0].Trim().ToCharArray(), long.Parse(sArr[1])))
            .Select(constructHand)
            .ToArray();

        Array.Sort(input);

        return input.Select((hand, i) => (hand, i))
            .Aggregate(
                0L,
                (sum, tuple) => sum + tuple.hand.Bid * (tuple.i + 1)
            );
    }


    internal class Hand : IComparable<Hand>
    {
        public readonly long Bid;
        private readonly char[] _cards;
        private readonly CardValue _cardValue;
        private readonly Func<char, int> _cardToValueFunction;

        internal static Hand ConstructHandForA((char[] cards, long bid) tuple)
            => new(AFunctions.CalcScoreForA(tuple.cards), tuple, AFunctions.CardToValueForA);

        internal static Hand ConstructHandForB((char[] cards, long bid) tuple)
            => new(BFunctions.CalcScoreForB(tuple.cards), tuple, BFunctions.CardToValueForB);


        private Hand(CardValue cardValue, (char[] cards, long bid) tuple, Func<char, int> cardToValueFunction)
        {
            _cardValue = cardValue;
            (_cards, Bid) = tuple;
            _cardToValueFunction = cardToValueFunction;
        }

        public int CompareTo(Hand? other)
        {
            if (other == null)
            {
                return 1;
            }

            var firstCompareResult = _cardValue.CompareTo(other._cardValue);
            if (firstCompareResult != 0)
            {
                return firstCompareResult;
            }

            return _cards.Select(_cardToValueFunction)
                .Zip(other._cards.Select(_cardToValueFunction))
                .Select(cardsToCompare => cardsToCompare.First.CompareTo(cardsToCompare.Second))
                .FirstOrDefault(result => result != 0, 0);
        }

        public override string ToString() =>
            $"[{string.Join(", ", _cards)}], {Bid}, {_cardValue.ToString()}={_cardValue.ToValue()}";
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

internal enum CardValue
{
    FiveOfAKind = 19,
    FourOfAKind = 18,
    FullHouse = 17,
    ThreeOfAKind = 16,
    TwoPair = 15,
    OnePair = 14,
    HighCard = 0,
}

internal static class CardValueMethods
{
    public static string ToString(this CardValue self)
    {
        return self switch
        {
            CardValue.FiveOfAKind => "Five of a kind",
            CardValue.FourOfAKind => "Four of a kind",
            CardValue.FullHouse => "Full house",
            CardValue.ThreeOfAKind => "Three of a kind",
            CardValue.TwoPair => "Two pair",
            CardValue.OnePair => "One pair",
            CardValue.HighCard => "High card",
            _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
        };
    }

    public static int ToValue(this CardValue self)
        => (int)self;
}

internal static class BFunctions
{
    internal static int CardToValueForB(char val)
    {
        return val switch
        {
            'A' => 13,
            'K' => 12,
            'Q' => 11,
            'T' => 10,
            '9' => 9,
            '8' => 8,
            '7' => 7,
            '6' => 6,
            '5' => 5,
            '4' => 4,
            '3' => 3,
            '2' => 2,
            'J' => 1,
            _ => throw new ArgumentException("Invalid card encountered")
        };
    }

    internal static CardValue CalcScoreForB(char[] cards)
    {
        Dictionary<char, int> cardCount = new();
        foreach (var card in cards)
        {
            cardCount[card] = cardCount.GetValueOrDefault(card, 0) + 1;
        }

        if (cardCount.ContainsValue(5))
        {
            return CardValue.FiveOfAKind;
        }

        if (cardCount.ContainsValue(4))
        {
            return cardCount.ContainsKey('J') ? CardValue.FiveOfAKind : CardValue.FourOfAKind;
        }

        if (cardCount.ContainsValue(3) && cardCount.ContainsValue(2))
        {
            return cardCount.ContainsKey('J') ? CardValue.FiveOfAKind : CardValue.FullHouse;
        }

        if (cardCount.ContainsValue(3))
        {
            return cardCount.ContainsKey('J') ? CardValue.FourOfAKind : CardValue.ThreeOfAKind;
        }

        if (cardCount.ContainsValue(2) && cardCount.Values.Count == 3)
        {
            return cardCount.GetValueOrDefault('J', 0) switch
            {
                2 => CardValue.FourOfAKind,
                1 => CardValue.FullHouse,
                0 => CardValue.TwoPair,
                _ => throw new ArgumentException("Joker count is invalid")
            };
        }

        if (cardCount.ContainsValue(2)) // && cardCountValues.Count == 4
        {
            return cardCount.ContainsKey('J') ? CardValue.ThreeOfAKind : CardValue.OnePair;
        }

        return cardCount.ContainsKey('J') ? CardValue.OnePair : CardValue.HighCard;
    }
}

internal static class AFunctions
{
    internal static int CardToValueForA(char val)
    {
        return val switch
        {
            'A' => 13,
            'K' => 12,
            'Q' => 11,
            'J' => 10,
            'T' => 9,
            '9' => 8,
            '8' => 7,
            '7' => 6,
            '6' => 5,
            '5' => 4,
            '4' => 3,
            '3' => 2,
            '2' => 1,
            _ => throw new ArgumentException("Invalid card encountered")
        };
    }

    internal static CardValue CalcScoreForA(char[] cards)
    {
        Dictionary<char, int> cardCount = new();
        foreach (var card in cards)
        {
            cardCount[card] = cardCount.GetValueOrDefault(card, 0) + 1;
        }

        // Five of a kind, where all five cards have the same label: AAAAA
        if (cardCount.ContainsValue(5))
        {
            return CardValue.FiveOfAKind;
        }

        // Four of a kind, where four cards have the same label and one card has a different label: AA8AA
        if (cardCount.ContainsValue(4))
        {
            return CardValue.FourOfAKind;
        }

        // Full house, where three cards have the same label, and the remaining two cards share a different label: 23332
        if (cardCount.ContainsValue(3) && cardCount.ContainsValue(2))
        {
            return CardValue.FullHouse;
        }

        // Three of a kind, where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
        if (cardCount.ContainsValue(3))
        {
            return CardValue.ThreeOfAKind;
        }

        // Two pair, where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
        if (cardCount.ContainsValue(2) && cardCount.Values.Count == 3)
        {
            return CardValue.TwoPair;
        }

        // One pair, where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
        if (cardCount.ContainsValue(2)) // && cardCountValues.Count == 4
        {
            return CardValue.OnePair;
        }

        // High card, where all cards' labels are distinct: 23456
        return CardValue.HighCard;
    }
}