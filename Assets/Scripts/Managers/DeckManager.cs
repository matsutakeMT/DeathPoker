using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private Queue<Card> deck;
    private int nextCardId;
    private SealSettings settings;

    public DeckManager(SealSettings _settings)
    {
        settings = _settings;
    }

    public void CreateDeck()
    {
        deck.Clear();
        nextCardId = 0;
        List<Card> cards = CreateNormalCards();
        List<Card> jokers = CreateJokers(2);
        cards.AddRange(jokers);
        Shuffle(cards);
        deck = new(cards);
    }

    private List<Card> CreateNormalCards()
    {
        Suit[] suits =
        {
            Suit.Club,
            Suit.Diamond,
            Suit.Heart,
            Suit.Spade
        };

        List<Card> deck = new(54);
        foreach (Suit suit in suits)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card card = new Card();

                card.CardId = nextCardId++;
                card.Suit = suit;
                card.Rank = rank;
                card.IsJoker = false;

                AssignSeal(card);
                deck.Add(card);
            }
        }
        return deck;
    }

    private List<Card> CreateJokers(int count)
    {
        List<Card> cards = new(count);
        for (int i = 0; i < count; i++)
        {
            Card joker = new Card();

            joker.CardId = nextCardId++;
            joker.Suit = Suit.Joker;
            joker.Rank = 0;
            joker.IsJoker = true;

            AssignSeal(joker);
            cards.Add(joker);
        }
        return cards;
    }

    private void AssignSeal(Card card)
    {
        float randValue = Random.value;

        if (randValue > settings.ProbabilityDeathSeal) return;

        if (randValue < settings.ProbabilityDeath1)
        {
            card.Seal = new DeathSeal(SealType.Death1);
            return;
        }

        if (randValue < settings.ProbabilityDeath1 + settings.ProbabilityDeath3)
        {
            card.Seal = new DeathSeal(SealType.Death3);
            return;
        }

        else
        {
            card.Seal = new DeathSeal(SealType.Death5);
            return;
        }
    }

    private void Shuffle(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
    }

    public Card Draw()
    {
        if (deck.TryDequeue(out Card card))
        {
            return card;
        }
        Debug.LogError("Deck is empty.");
        return null;
    }

    public int Count()
    {
        return deck.Count;
    }

    public Queue<Card> GetAllCards()
    {
        return deck;
    }
}