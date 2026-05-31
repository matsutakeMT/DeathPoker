using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private List<Card> deck;

    private int nextCardId;

    private const float Death1Rate = 2f;
    private const float Death3Rate = 10f;
    private const float Death5Rate = 20f;

    public DeckManager()
    {
        deck = new List<Card>();
    }

    public void CreateDeck()
    {
        deck.Clear();

        nextCardId = 0;

        CreateNormalCards();

        CreateJokers();

        Shuffle();
    }

    private void CreateNormalCards()
    {
        Suit[] suits =
        {
            Suit.Club,
            Suit.Diamond,
            Suit.Heart,
            Suit.Spade
        };

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
    }

    private void CreateJokers()
    {
        for (int i = 0; i < 2; i++)
        {
            Card joker = new Card();

            joker.CardId = nextCardId++;
            joker.Suit = Suit.Joker;
            joker.Rank = 0;
            joker.IsJoker = true;

            AssignSeal(joker);

            deck.Add(joker);
        }
    }

    private void AssignSeal(Card card)
    {
        float roll = Random.Range(0f, 100f);

        if (roll < Death1Rate)
        {
            card.Seal =
                new DeathSeal(
                    SealType.Death1);

            return;
        }

        if (roll < Death1Rate + Death3Rate)
        {
            card.Seal =
                new DeathSeal(
                    SealType.Death3);

            return;
        }

        if (roll <
            Death1Rate +
            Death3Rate +
            Death5Rate)
        {
            card.Seal =
                new DeathSeal(
                    SealType.Death5);

            return;
        }

        card.Seal = null;
    }

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    deck.Count);

            (deck[i], deck[randomIndex]) =
                (deck[randomIndex], deck[i]);
        }
    }

    public Card Draw()
    {
        if (deck.Count == 0)
        {
            Debug.LogError(
                "Deck is empty.");

            return null;
        }

        Card card = deck[0];

        deck.RemoveAt(0);

        return card;
    }

    public int Count()
    {
        return deck.Count;
    }

    public List<Card> GetAllCards()
    {
        return deck;
    }
}