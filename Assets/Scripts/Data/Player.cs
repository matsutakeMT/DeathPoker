using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string Name { get; private set; }
    public int Chips { get; private set; }
    public int CurrentBet { get; private set; }
    public List<Card> Hand { get; private set; } = new(2);
    public HashSet<int> ObservedCards { get; private set; } = new(23);
    
    // 状態
    public bool IsDead { get; private set; }
    public bool IsFolded { get; private set; }
    public bool IsBankrupt
    {
        get
        {
            return Chips <= 0;
        }
    }
    public bool HasBeenDealer { get; private set; }

    public int Death1Count;
    public int Death3Count;
    public int Death5Count;

    public Player(string name, int chips)
    {
        Name = name;
        Chips = chips;
        ResetRound();
    }

    public bool CanBet(int amount)
    {
        return Chips >= amount;
    }

    public void ResetRound()
    {
        IsDead = false;
        IsFolded = false;

        CurrentBet = 0;

        Death1Count = 0;
        Death3Count = 0;
        Death5Count = 0;

        Hand.Clear();
        ObservedCards.Clear();
    }

    public bool Bet(int amount)
    {
        if (amount > Chips)
            return false;

        Chips -= amount;
        CurrentBet += amount;

        return true;
    }
}
