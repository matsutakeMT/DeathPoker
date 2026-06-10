public class SealManager
{
    private AudioManager audioManager;

    public SealManager(AudioManager _audioManager)
    {
        audioManager = _audioManager;
    }

    public ObserveResult ObserveCard(Player player, Card card)
    {
        if (player == null)
            return new ObserveResult();

        if (card.Seal == null)
        {
            return new ObserveResult
            {
                Observed = true
            };
        }

        if (player.ObservedCards.Contains(card.CardId))
        {
            return new ObserveResult();
        }

        player.ObservedCards.Add(card.CardId);

        AddSealCount(player, card.Seal);

        bool died = CheckDeath(player);

        if (died)
        {
            Kill(player);

            return new ObserveResult
            {
                Observed = true,
                Died = true,
                CauseCard = card,
                CauseSeal = card.Seal
            };
        }

        return new ObserveResult
        {
            Observed = true,
            Died = false
        };
    }

    private void AddSealCount(Player player, DeathSeal seal)
    {
        switch (seal.SealType)
        {
            case SealType.Death1:
                player.Death1Count++;
                break;

            case SealType.Death3:
                player.Death3Count++;
                break;

            case SealType.Death5:
                player.Death5Count++;
                break;
        }
    }

    private bool CheckDeath(Player player)
    {
        if (player.Death1Count >= 1)
            return true;

        if (player.Death3Count >= 3)
            return true;

        if (player.Death5Count >= 5)
            return true;

        return false;
    }

    private void Kill(Player player)
    {
        audioManager.SoundScream();
        player.IsDead = true;
        player.IsFolded = true;
    }
}