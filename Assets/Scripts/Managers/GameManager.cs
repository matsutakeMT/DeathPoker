using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(2, 9)]
    [SerializeField] public int playerCount = 5;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SealSettings sealSettings;

    private List<Player> players = new();
    private List<Card> communityCards = new();
    private HandEvaluator handEvaluator;
    private DeckManager deckManager;
    private SealManager sealManager;
    private CpuManager cpuManager;
    private BettingManager bettingManager;

    private int currentDealerIndex;
    private bool deathEveryone;

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

        for (int i = 1; i <= playerCount; i++)
        {
            Player player = new Player($"Player{i}", 1000);
            players.Add(player);
        }

        Debug.Log($"Players : {playerCount}");
    }

    private void StartRound()
    {
        communityCards.Clear();

        uiManager.HideWinner();
        uiManager.HideDeath();
        deathEveryone = false;

        bettingManager = new BettingManager();
        cpuManager = new CpuManager();
        sealManager = new SealManager(audioManager);
        handEvaluator = new HandEvaluator();
        deckManager = new DeckManager(sealSettings);
        uiManager.RefreshCommunity();

        Debug.Log("=== ROUND START ===");

        foreach (Player player in players)
        {
            player.ResetRound();
        }

        deckManager.CreateDeck();
        StartCoroutine(RoundRoutine());
    }

    private void DealCards()
    {
        int deathPlayerCount = 0;
        foreach (Player player in players)
        {
            for (int i = 0; i < 2; i++)
            {
                if (player.IsDead)
                    break;
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
                    deathPlayerCount++;
                    Debug.Log($"{player.Name} Died");
                    uiManager.RefreshPlayers();

                    if (player == players[0])
                    {
                        uiManager.ShowDeath(result.CauseCard);
                    }
                }
            }
        }

        Debug.Log(deathPlayerCount);
        if (deathPlayerCount == playerCount)
            deathEveryone = true;
    }

    /// <summary>
    /// Flop, Turn, Riverを行うメソッド
    /// </summary>
    /// <param name="revealCount">めくるコミュニティカードの枚数</param>
    private void RevealCommunity(int revealCount)
    {
        Debug.Log($"=== Reveal {revealCount} ===");
        int deathPlayerCount = 0;
        for (int i = 0; i < revealCount; i++)
        {
            Card card = deckManager.Draw();
            communityCards.Add(card);
            deathPlayerCount += ObserveCommunityCard(card);
        }
        uiManager.RefreshCommunity();
        if (deathPlayerCount == playerCount)
            deathEveryone = true;
    }

    private int ObserveCommunityCard(Card card)
    {
        int deathPlayerCount = 0;
        foreach (Player player in players)
        {
            if (player.IsDead) continue;

            ObserveResult result = sealManager.ObserveCard(player, card);

            if (player == players[0])
            {
                uiManager.RefreshDeathCounts();
            }

            if (result.Died)
            {
                deathPlayerCount++;
                Debug.Log($"{player.Name} Died");
                uiManager.RefreshPlayers();

                if (player == players[0])
                {
                    uiManager.ShowDeath(result.CauseCard);
                }
            }
            Debug.Log($"{player.Name} " + $"D1={player.Death1Count} " + $"D3={player.Death3Count} " + $"D5={player.Death5Count}");
        }
        return deathPlayerCount;
    }

    private void Showdown()
    {
        uiManager.ShowdownReveal();
        Dictionary<string, HandResult> handResults = new(playerCount);

        Debug.Log("=== SHOWDOWN ===");
        foreach (Player player in players)
        {
            Debug.Log($"{player.Name} "
                + $"\nBet={player.CurrentBet}" + $"Chips={player.Chips} "
                + $"\n{player.Death1Count}" + $"/{player.Death3Count}" + $"/{player.Death5Count}");
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

    private IEnumerator OnEveryoneDeadRoutine()
    {
        Debug.Log("Round Invalid");
        if (deathEveryone)
            yield return StartCoroutine(uiManager.ColorizeDeath());
        EndRound();
        yield break;
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

            if (currentDealerIndex >= playerCount)
            {
                currentDealerIndex = 0;
            }

            if (!players[currentDealerIndex].IsBankrupt)
            {
                Debug.Log($"Dealer : {players[currentDealerIndex].Name}");
                return;
            }
        }
        while (currentDealerIndex != startIndex);
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
        RunCpuTurns();

        uiManager.RefreshPlayers();
        uiManager.RefreshDeathCounts();

        if (IsEveryoneDead())
        {
            StartCoroutine(OnEveryoneDeadRoutine());
            yield break;
        }
        yield return new WaitForSeconds(1f);

        int[] revealCounts = new int[] { 3, 1, 1 };
        foreach (int count in revealCounts)
        {
            RevealCommunity(count);
            if (IsEveryoneDead())
            {
                StartCoroutine(OnEveryoneDeadRoutine());
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }

        Showdown();
        yield return new WaitForSeconds(3f);
        EndRound();
    }

    private void RunCpuTurns()
    {
        Debug.Log("RUN CPU TURNS");
        for (int i = 1; i < playerCount; i++)
        {
            Debug.Log($"CPU Loop {i}");
            Player player = players[i];
            if (player.IsBankrupt || player.IsDead) continue;
            CpuAction action = cpuManager.ExecuteTurn(player, 0, bettingManager.Pot);

            switch (action)
            {
                case CpuAction.Fold:
                    bettingManager.Fold(player);
                    break;

                case CpuAction.Check:
                    bettingManager.Check(player);
                    break;

                case CpuAction.Call:
                    bettingManager.Call(player);
                    break;

                case CpuAction.Raise:
                    bettingManager.Raise(player, 10);
                    break;
            }

            uiManager.RefreshPot(bettingManager.Pot);

            Debug.Log($"{player.Name} : {action}");
            Debug.Log($"{player.Name} " + $"Chips={player.Chips}");
            Debug.Log($"CurrentBet = " + $"{bettingManager.CurrentBet}");

        }
    }
}