using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(2, 9)]
    public int playerCount = 5;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SealSettings sealSettings;

    private List<Player> players = new();
    private List<Card> communityCards = new();
    private HandEvaluator handEvaluator;
    private DeckManager deckManager;
    private SealManager sealManager;
    private NpcManager npcManager;
    private BettingManager bettingManager;

    /// <summary>
    /// 現在のディーラーを示す
    /// </summary>
    private int currentDealerIndex;
    /// <summary>
    /// 全員が一度に死んだかを持つ変数<br/>
    /// </summary>
    // 変数名が分かりにくい?
    private bool deathEveryone;

    public IReadOnlyList<Player> Players => players;
    public IReadOnlyList<Card> CommunityCards => communityCards;

    private void Start()
    {
        StartGame();
    }

    // hardcoded:
    //   最初のディーラー: 0, 最初のラウンド開始までの待機秒数: 2f
    /// <summary>
    /// ゲーム全体の初期化と最初のラウンドの予約を行うメソッド
    /// </summary>
    private void StartGame()
    {
        CreatePlayers();
        currentDealerIndex = 0;
        uiManager.Initialize();
        Invoke(nameof(StartRound), 2f);
    }

    // hardcoded:
    //   プレイヤーの初期値: {name: "Player{i}", chips: 1000}
    // log:
    //   プレイヤー数
    /// <summary>
    /// プレイヤーを人数分作成してplayersに追加
    /// プレイヤー初期値は {name: Player{i}, 1000}
    /// </summary>
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

    // hardcoded:
    //   ante: 10
    // log:
    //   ラウンドスタート
    /// <summary>
    /// ラウンドの初期化を行うメソッド
    /// </summary>
    private void StartRound()
    {
        communityCards.Clear();

        uiManager.HideWinner();
        uiManager.HideDeath();
        deathEveryone = false;

        bettingManager = new BettingManager();
        npcManager = new NpcManager();
        sealManager = new SealManager(audioManager);
        handEvaluator = new HandEvaluator();
        deckManager = new DeckManager(sealSettings);

        bettingManager.ResetRound();
        bettingManager.CollectAnte(players, 10);
        uiManager.RefreshPot(bettingManager.Pot);
        uiManager.RefreshCommunity();

        Debug.Log("=== ROUND START ===");

        foreach (Player player in players)
        {
            player.ResetRound();
        }

        deckManager.CreateDeck();
        StartCoroutine(RoundRoutine());
    }

    // log:
    //   死亡プレイヤー名
    /// <summary>
    /// 各プレイヤーにカードを配るメソッド
    /// デスの確認，デスカウント・デスパネルのUI編集，全員デスの確認も行う
    /// </summary>
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

    // log:
    //   実行開始時ログ
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
        }
        return deathPlayerCount;
    }

    // log:
    //   実行開始時ログ, 各プレイヤーの状態, 勝者
    /// <summary>
    /// ショーダウン
    /// </summary>
    private void Showdown()
    {
        uiManager.ShowdownReveal();
        Dictionary<string, HandResult> handResults = new(playerCount);

        Debug.Log("=== SHOWDOWN ===");
        foreach (Player player in players)
        {
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
            bettingManager.AwardPot(winner);
            uiManager.ShowWinner(winner.Name, maxValue.HandRank.ToString());
        }
        else
        {
            uiManager.ShowWinner("DRAW", maxValue.HandRank.ToString());
        }
    }

    /// <summary>
    /// 全員が死んでいる状態か判定するメソッド
    /// </summary>
    private bool IsEveryoneDead()
    {
        foreach (Player player in players)
        {
            if (!player.IsDead)
                return false;
        }
        return true;
    }

    // log:
    //   "Round Invalid"
    /// <summary>
    /// 全員が死んでいる状態のときに実行するメソッド<br />
    /// 全員が一度に死んだとき虹演出を実行する
    /// </summary>
    private IEnumerator OnEveryoneDeadRoutine()
    {
        Debug.Log("Round Invalid");
        if (deathEveryone)
            yield return StartCoroutine(uiManager.ColorizeDeath());
        EndRound();
        yield break;
    }

    // hardcoded:
    //   次のラウンド開始前の待機時間: 1f
    // log:
    //   ラウンド終了ログ
    /// <summary>
    /// ラウンド終了時ゲーム終了判定するメソッド
    /// </summary>
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

    // 次のディーラーが見つからなかったときバグが発生しそう?
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

    // hardcoded:
    //   各行動後の待機時間: 1f | 3f
    /// <summary>
    /// ラウンドを進めるメソッド
    /// </summary>
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

    // ! RunCpuTurns -> RunNpcTurnsに変更すべき
    // ! 必ず1->最後で1順だけ実行している
    // ! プレイヤーはこのメソッドで実行しない
    // ! 正しくかけられたか判定していない
    // hardcoded:
    //   NPCプレイヤーの現在のベット額: 0, レイズ額: 10
    // log:
    //   実行開始時ログ, 実行中Npcインデックス
    /// <summary>
    /// NPCのターンを実行するメソッド
    /// </summary>
    private void RunCpuTurns()
    {
        Debug.Log("RUN CPU TURNS");
        for (int i = 1; i < playerCount; i++)
        {
            Debug.Log($"CPU Loop {i}");
            Player player = players[i];
            if (player.IsBankrupt || player.IsDead) continue;
            NpcAction action = npcManager.ExecuteTurn(player, 0, bettingManager.Pot);

            switch (action)
            {
                case NpcAction.Fold:
                    bettingManager.Fold(player);
                    break;

                case NpcAction.Check:
                    bettingManager.Check(player);
                    break;

                case NpcAction.Call:
                    bettingManager.Call(player);
                    break;

                case NpcAction.Raise:
                    bettingManager.Raise(player, 10);
                    break;
            }

            uiManager.RefreshPot(bettingManager.Pot);

        }
    }
}