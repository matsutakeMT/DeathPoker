using System;

[Serializable]
public class Card
{
    public int CardId;

    public Suit Suit;

    public int Rank;

    public bool IsJoker;

    public DeathSeal Seal;

    public string SpriteName
    {
        get
        {
            if (IsJoker)
                return "TrumpJoker";

            string suit =
                Suit switch
                {
                    Suit.Club => "C",
                    Suit.Diamond => "D",
                    Suit.Heart => "H",
                    Suit.Spade => "S",
                    _ => ""
                };

            return $"Trump{suit}{Rank}";
        }
    }
}
