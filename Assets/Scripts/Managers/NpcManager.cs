using UnityEngine;

public class NpcManager
{
    public NpcAction ExecuteTurn(Player player, int currentBet, int pot)
    {
        int random = Random.Range(0, 4);
        return (NpcAction)random;
    }
}