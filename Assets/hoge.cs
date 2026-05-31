using UnityEngine;

public class DeckTest : MonoBehaviour
{
    private void Start()
    {
        DeckManager deck =
            new DeckManager();

        deck.CreateDeck();

        Debug.Log(
            $"Deck Count : {deck.Count()}");

        Card card =
            deck.Draw();

        Debug.Log(
            $"Draw : {card.Suit} {card.Rank}");

        Debug.Log(
            $"Deck Count : {deck.Count()}");
    }
}