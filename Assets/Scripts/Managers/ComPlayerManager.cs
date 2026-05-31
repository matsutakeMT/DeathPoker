using UnityEngine;

public class ComPlayerManager
{
    public ComPlayerAction ExecuteTurn(
    Player player)
    {
        int random =
        Random.Range(0, 4);

        return (ComPlayerAction)random;
    }
}