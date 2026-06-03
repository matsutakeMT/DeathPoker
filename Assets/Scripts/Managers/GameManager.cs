using PokerGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SealSettings sealSettings;
    private List<Player> players = new();

    private List<Card> communityCards = new();

    private HandEvaluator handEvaluator;

    private DeckManager deckManager;

    private SealManager sealManager;

    private int currentDealerIndex;

    public IReadOnlyList<Player> Players => players;

    public IReadOnlyList<Card> CommunityCards => communityCards;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        CreatePlayers();

        currentDealerIndex = 0;

        Debug.Log($"Dealer : {players[currentDealerIndex].Name}");

        uiManager.Initialize();

        Invoke(nameof(StartRound), 2f);
    }

    private void CreatePlayers()
    {
        players.Clear();

        for (int i = 1; i <= 5; i++)
        {
            Player player = new Player($"Player{i}", 1000);

            players.Add(player);
        }

        Debug.Log($"Players : {players.Count}");
    }

    private void StartRound()
    {
        communityCards.Clear();

        uiManager.HideWinner();
        uiManager.HideDeath();
        uiManager.RefreshCommunity();

        Debug.Log("=== ROUND START ===");

        foreach (Player player in players)
        {
            player.ResetRound();
        }

        deckManager = new DeckManager(sealSettings);
        deckManager.CreateDeck();

        sealManager = new SealManager(audioManager);

        handEvaluator = new HandEvaluator();

        StartCoroutine(RoundRoutine());
    }

    private void DealCards()
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                Card card = deckManager.Draw();

                player.Hand.Add(card);

                ObserveResult result = sealManager.ObserveCard(player, card);

                if (player == players[0])
                {
                    uiManager.RefreshDeathCounts();
                }

                Debug.Log($"{player.Name} Draw");

                if (result.Died)
                {
                    Debug.Log($"{player.Name} Died");

                    uiManager.RefreshPlayers();

                    if (player == players[0])
                    {
                        uiManager.ShowDeath(result.CauseCard);
                    }
                }
            }
        }
    }

    private void RevealFlop()
    {
        Debug.Log("=== FLOP ===");

        for (int i = 0; i < 3; i++)
        {
            Card card = deckManager.Draw();

            communityCards.Add(card);

            ObserveCommunityCard(card);
        }
        uiManager.RefreshCommunity();

        Debug.Log($"Community Count = {communityCards.Count}");
        Debug.Log("CALL RefreshCommunity");
    }

    private void RevealTurn()
    {
        Debug.Log("=== TURN ===");

        Card card = deckManager.Draw();

        communityCards.Add(card);

        ObserveCommunityCard(card);

        uiManager.RefreshCommunity();
    }

    private void RevealRiver()
    {
        Debug.Log("=== RIVER ===");

        Card card = deckManager.Draw();

        communityCards.Add(card);

        ObserveCommunityCard(card);

        uiManager.RefreshCommunity();
    }

    private void ObserveCommunityCard(Card card)
    {
        foreach (Player player in players)
        {
            if (player.IsDead)
                continue;

            ObserveResult result = sealManager.ObserveCard(player, card);

            if (player == players[0])
            {
                uiManager.RefreshDeathCounts();
            }

            if (result.Died)
            {
                Debug.Log($"{player.Name} Died");

                uiManager.RefreshPlayers();

                if (player == players[0])
                {
                    uiManager.ShowDeath(result.CauseCard);
                }
            }
            Debug.Log($"{player.Name} " + $"D1={player.Death1Count} " + $"D3={player.Death3Count} " + $"D5={player.Death5Count}");
        }
    }

    private void Showdown()
    {
        uiManager.ShowdownReveal();

        Dictionary<string, HandResult> handResults = new(players.Count);

        Debug.Log("=== SHOWDOWN ===");

        foreach (Player player in players)
        {
            Debug.Log($"{player.Name}" + $" {player.Death1Count}" + $"/{player.Death3Count}" + $"/{player.Death5Count}");
            if (player.IsDead)
            {
                Debug.Log($"{player.Name} DEAD");

                continue;
            }
            Debug.Log($"{player.Name} ALIVE");

            HandResult r = handEvaluator.Execute(communityCards.Concat(player.Hand).ToList());
            handResults.Add(player.Name, r);
        }

        HandResult maxValue = handResults.Values.Max();
        KeyValuePair<string, HandResult>[] maxValuePlayers = handResults.Where(kvp => kvp.Value == maxValue).ToArray();

        Player winner = maxValuePlayers.Length == 1 ? players.Find(p => p.Name == maxValuePlayers[0].Key) : null;
        Debug.Log($"{(winner is null ? "draw" : winner.Name)}");

        if (winner != null)
        {
            uiManager.ShowWinner(winner.Name, maxValue.HandRank.ToString());
        }
        else
        {
            uiManager.ShowWinner("DRAW", maxValue.HandRank.ToString());
        }
    }

    private bool IsEveryoneDead()
    {
        foreach (Player player in players)
        {
            if (!player.IsDead)
                return false;
        }

        return true;
    }

    private void EndRound()
    {
        Debug.Log("=== ROUND END ===");

        players[currentDealerIndex].HasBeenDealer = true;

        if (CheckGameEnd())
        {
            EndGame();
            return;
        }

        MoveDealer();

        Invoke(nameof(StartRound), 1f);
    }

    private void MoveDealer()
    {
        int startIndex = currentDealerIndex;

        do
        {
            currentDealerIndex++;

            if (currentDealerIndex >= players.Count)
            {
                currentDealerIndex = 0;
            }

            if (!players[currentDealerIndex].IsBankrupt)
            {

                Debug.Log($"Dealer : {players[currentDealerIndex].Name}");

                return;
            }

        }
        while (
            currentDealerIndex != startIndex);
    }

    private bool CheckGameEnd()
    {
        foreach (Player player in players)
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
        Debug.Log("===== GAME END =====");
    }

    private IEnumerator RoundRoutine()
    {
        DealCards();

        uiManager.RefreshPlayers();
        uiManager.RefreshDeathCounts();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        RevealFlop();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        RevealTurn();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        RevealRiver();

        if (IsEveryoneDead())
        {
            Debug.Log("Round Invalid");
            EndRound();
            yield break;
        }

        yield return new WaitForSeconds(1f);

        Showdown();

        yield return new WaitForSeconds(3f);

        EndRound();
    }
}