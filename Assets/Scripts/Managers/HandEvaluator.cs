using System;
using System.Collections.Generic;
using System.Linq;


public class HandEvaluator
{

    /// <summary>
    /// カードから最も強い役を返す関数
    /// </summary>
    /// <param name="cards">7枚のカードリスト</param>
    /// <returns>判定した結果</returns>
    public HandResult Execute(List<Card> cards)
    {
        List<HandResult> result = new(21);

        var jokerCount = cards.Count(c => c.IsJoker);
        if (jokerCount > 0)
        {
            var remaining = cards.Where(c => c != cards.First(c => c.IsJoker)).ToList();

            List<Card> jokerPlaceholders = GetNormalCards();
            foreach (var placeholder in jokerPlaceholders)
            {
                IEnumerable<Card> replaceCards = remaining.Append(placeholder);
                result.Add(Execute(replaceCards.ToList()));
            }
        }
        else
        {
            foreach (var cards5 in cards.Combinations(5))
                result.Add(EvaluateFrom5Cards(cards5));
        }
        return result.Max();
    }
    private List<Card> GetNormalCards()
    {
        List<Card> cards = new(4 * 13);
        foreach (object s in Enum.GetValues(typeof(Suit)))
        {
            Suit suit = (Suit)s;
            for (var rank = 1; rank <= 13; rank++)
            {
                Card card = new Card();
                card.Suit = suit;
                card.Rank = rank;
                card.IsJoker = false;

                cards.Add(card);
            }
        }
        return cards;
    }

    private HandResult EvaluateFrom5Cards(IEnumerable<Card> cards)
    {
        // 枚数 → ランクのリスト（逆引き用）lookup

        Dictionary<int, int> rankCounts = GetUniqueCounts(cards);
        ILookup<int, int> lookup = rankCounts.ToLookup(x => x.Value, x => x.Key);

        bool isStraight = IsStraight(cards);
        bool isFlush = IsFlush(cards);

        int rankMax = rankCounts.Keys.Max();

        List<int> tiebreakers = new(5);
        // == 判定開始
        if (lookup[5].Any())
        {
            tiebreakers.Add(lookup[5].First());
            return new(HandRank.FiveOfAKind, tiebreakers);
        }

        if (isStraight && isFlush)
        {
            if (rankMax == 14 && rankCounts.ContainsKey(2))
                rankMax = 5;
            tiebreakers.Add(rankMax);
            return rankMax == 14 ?
                new(HandRank.RoyalStraightFlush, Array.Empty<int>()) :
                new(HandRank.StraightFlush, tiebreakers);
        }
        if (lookup[4].Any())
        {
            tiebreakers.Add(lookup[4].First());
            tiebreakers.Add(lookup[1].First());
            return new(HandRank.FourOfAKind, tiebreakers);
        }
        if (lookup[3].Any() && lookup[2].Any())
        {
            tiebreakers.Add(lookup[3].First());
            tiebreakers.Add(lookup[2].First());
            return new(HandRank.FullHouse, tiebreakers);
        }
        if (isFlush)
        {
            var ordered = cards
                .Select(c => c.Rank == 1 ? 14 : c.Rank)
                .OrderByDescending(n => n);
            foreach (var item in ordered)
                tiebreakers.Add(item);
            return new(HandRank.Flush, tiebreakers);
        }
        if (isStraight)
        {
            if (rankMax == 14 && rankCounts.ContainsKey(2))
                rankMax = 5;
            tiebreakers.Add(rankMax);
            return new(HandRank.Straight, tiebreakers);
        }
        if (lookup[3].Any())
        {
            tiebreakers.Add(lookup[3].First());
            foreach (var i in lookup[1].OrderByDescending(n => n))
                tiebreakers.Add(i);
            return new(HandRank.ThreeOfAKind, tiebreakers);
        }
        switch (lookup[2].Count())
        {
            case 2:
                foreach (var i in lookup[2].OrderByDescending(n => n))
                    tiebreakers.Add(i);
                tiebreakers.Add(lookup[1].First());
                return new(HandRank.TwoPair, tiebreakers);
            case 1:
                tiebreakers.Add(lookup[2].First());
                foreach (var i in lookup[1].OrderByDescending(n => n))
                    tiebreakers.Add(i);
                return new(HandRank.OnePair, tiebreakers);
            default:
                break;
        }
        foreach (var i in lookup[1].OrderByDescending(n => n))
            tiebreakers.Add(i);
        return new(HandRank.HighCard, tiebreakers);
    }

    private Dictionary<int, int> GetUniqueCounts(IEnumerable<Card> cards)
    {
        Dictionary<int, int> counts = new();
        foreach (var c in cards)
            counts[c.Rank] = counts.ContainsKey(c.Rank) ? counts[c.Rank] + 1 : 1;
        if (counts.TryGetValue(1, out int aceCount))
            counts[14] = aceCount;
        counts.Remove(1);
        return counts;
    }

    private bool IsStraight(IEnumerable<Card> cards)
    {
        int bits = 0;
        foreach (var c in cards) bits |= (1 << c.Rank);
        // A=1の時，14の判定を追加する
        if ((bits & (1 << 1)) != 0) bits |= (1 << 14);
        // 01111100 => 00111100 => 00011100 => 00001100 => 00000100 != 0
        return (bits & (bits >> 1) & (bits >> 2) & (bits >> 3) & (bits >> 4)) != 0;
    }

    private bool IsFlush(IEnumerable<Card> cards)
    {
        return cards.All(c => c.Suit == cards.First().Suit);
    }
}