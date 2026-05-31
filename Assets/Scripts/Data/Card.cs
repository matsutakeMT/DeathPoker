public class Card
{
    public int CardId { get; private set; }

    public Suit Suit { get; private set; }

    public int Rank { get; private set; }

    public bool IsJoker { get; private set; }

    public DeathSeal Seal { get; private set; }

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
