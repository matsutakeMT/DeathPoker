using PokerGame;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Player> players =
        new();

    private List<Card> communityCards =
        new();

    private DeckManager deckManager;

    private SealManager sealManager;

    private int currentDealerIndex;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        CreatePlayers();

        currentDealerIndex = 0;

        Debug.Log(
            $"Dealer : {players[currentDealerIndex].Name}");

        StartRound();
    }

    private void CreatePlayers()
    {
        players.Clear();

        for (int i = 1; i <= 5; i++)
        {
            Player player =
                new Player(
                    $"Player{i}",
                    1000);

            players.Add(player);
        }

        Debug.Log(
            $"Players : {players.Count}");
    }

    private void StartRound()
    {
        Debug.Log("=== ROUND START ===");

        communityCards.Clear();

        foreach (Player player in players)
        {
            player.ResetRound();
        }

        deckManager = new DeckManager();
        deckManager.CreateDeck();

        sealManager = new SealManager();

        DealCards();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            return;
        }

        RevealFlop();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            return;
        }

        RevealTurn();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            return;
        }

        RevealRiver();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            return;
        }

        Showdown();

        EndRound();
    }

    private void DealCards()
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                Card card =
                    deckManager.Draw();

                player.Hand.Add(card);

                ObserveResult result =
                    sealManager
                    .ObserveCard(
                        player,
                        card);

                Debug.Log(
                    $"{player.Name} Draw");

                if (result.Died)
                {
                    Debug.Log(
                        $"{player.Name} Died");

                    break;
                }
            }
        }
    }

    private void RevealFlop()
    {
        Debug.Log("=== FLOP ===");

        for (int i = 0; i < 3; i++)
        {
            Card card =
                deckManager.Draw();

            communityCards.Add(card);

            ObserveCommunityCard(card);
        }
    }

    private void RevealTurn()
    {
        Debug.Log("=== TURN ===");

        Card card =
            deckManager.Draw();

        communityCards.Add(card);

        ObserveCommunityCard(card);
    }

    private void RevealRiver()
    {
        Debug.Log("=== RIVER ===");

        Card card =
            deckManager.Draw();

        communityCards.Add(card);

        ObserveCommunityCard(card);
    }

    private void ObserveCommunityCard(
        Card card)
    {
        foreach (Player player
            in players)
        {
            if (player.IsDead)
                continue;

            ObserveResult result =
                sealManager
                .ObserveCard(
                    player,
                    card);

            if (result.Died)
            {
                Debug.Log(
                    $"{player.Name} Died");
            }
        }
    }

    private void Showdown()
    {
        Debug.Log(
            "=== SHOWDOWN ===");

        foreach (Player player
            in players)
        {
            Debug.Log(
                $"{player.Name}" +
                $" {player.Death1Count}" +
                $"/{player.Death3Count}" +
                $"/{player.Death5Count}");
            if (player.IsDead)
            {
                Debug.Log(
                    $"{player.Name} DEAD");

                continue;
            }

            Debug.Log(
                $"{player.Name} ALIVE");
        }
    }

    private bool IsEveryoneDead()
    {
        foreach (Player player
            in players)
        {
            if (!player.IsDead)
                return false;
        }

        return true;
    }

    private void EndRound()
    {
        Debug.Log(
            "=== ROUND END ===");

        players[currentDealerIndex]
            .HasBeenDealer = true;

        if (CheckGameEnd())
        {
            EndGame();
            return;
        }

        MoveDealer();

        StartRound();
    }

    private void MoveDealer()
    {
        int startIndex =
            currentDealerIndex;

        do
        {
            currentDealerIndex++;

            if (currentDealerIndex
                >= players.Count)
            {
                currentDealerIndex = 0;
            }

            if (!players[currentDealerIndex]
                .IsBankrupt)
            {

                Debug.Log(
                    $"Dealer : {players[currentDealerIndex].Name}");

                return;
            }

        }
        while (
            currentDealerIndex != startIndex);
    }

    private bool CheckGameEnd()
    {
        foreach (Player player
            in players)
        {
            if (player.IsBankrupt)
                continue;

            if (!player.HasBeenDealer)
                return false;
        }

        return true;
    }

    private void EndGame()
    {
        Debug.Log(
            "===== GAME END =====");
    }
}