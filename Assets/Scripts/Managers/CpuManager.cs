using UnityEngine;
public class CpuManager
{
    public CpuAction ExecuteTurn(Player player, int currentBet, int pot)
    {
        int random = Random.Range(0, 4);
        return (CpuAction)random;
    }
}