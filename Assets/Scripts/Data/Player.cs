using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string Name;
    public int Chips;
    public int CurrentBet;
    public List<Card> Hand  = new(2);
    public HashSet<int> ObservedCards  = new(23);
    
    // 状態
    public bool IsDead;
    public bool IsFolded;
    public bool IsBankrupt
    {
        get
        {
            return Chips <= 0;
        }
    }
    public bool HasBeenDealer;  

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
