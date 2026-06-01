using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugSample : MonoBehaviour
{
    // ヘルパー関数
    private Card C(
    Suit suit,
    int rank)
    {
        return new Card
        {
            Suit = suit,
            Rank = rank
        };
    }

    private Card Joker()
    {
        return new Card
        {
            IsJoker = true,
            Suit = Suit.Joker,
            Rank = 0
        };
    }

    // 変数
    HandEvaluator evaluator;

    public List<Card> cards;
    public HandRank expectRank;
    public List<int> expectTiebreakers;

    void Awake()
    {
    }

    void Start()
    {
        evaluator = new();

        var expect = new HandResult(expectRank, expectTiebreakers);
        var result = evaluator.Execute(cards);

        Assert.AreEqual(result.HandRank, expect.HandRank);
        Debug.Log(result.HandRank);
        Assert.AreEqual(result.Tiebreakers, expect.Tiebreakers);
        Debug.Log(string.Join(" ", result.Tiebreakers));

        Debug.Log("Done");
    }
}
