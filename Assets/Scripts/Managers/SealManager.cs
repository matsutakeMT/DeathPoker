namespace PokerGame
{
    public class SealManager
    {
        // ŠO•”ŒöŠJ
        public ObserveResult ObserveCard(Player player, Card card);

        // “à•”ˆ—
        private void AddSealCount(Player player, DeathSeal seal);

        private bool CheckDeath(Player player);

        private void Kill(Player player);
    }
}