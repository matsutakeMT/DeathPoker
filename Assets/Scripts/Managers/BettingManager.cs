using System.Collections.Generic;

public class BettingManager
{
    public int Pot
    {
        get;
        private set;
    }

    public void ResetRound()
    {
        Pot = 0;
    }

    public void CollectAnte(List<Player> players, int ante)
    {
        foreach (Player player in players)
        {
            if (player.IsBankrupt) continue;
            if (player.Bet(ante))
            {
                Pot += ante;
            }
        }
    }

    public void AwardPot(Player winner)
    {
        if (winner == null) return;

        winner.Chips += Pot;
        Pot = 0;
    }

    public void Fold(Player player)
    {
        player.IsFolded = true;
    }

    public void Check(Player player)
    {
    }

    public bool Call(Player player, int targetBet)
    {
        int diff = targetBet - player.CurrentBet;

        if (diff <= 0) return true;
        if (!player.Bet(diff)) return false;

        Pot += diff;

        return true;
    }

    public bool Raise(Player player, int amount)
    {
        if (!player.Bet(amount)) return false;
        Pot += amount;

        return true;
    }
}