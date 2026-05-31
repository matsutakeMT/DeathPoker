using UnityEngine;
using UnityEngine.Assertions;

public class DebugSample : MonoBehaviour
{
    ComPlayerManager playerManager;

    void Awake()
    {
        playerManager = new();
    }

    void Start()
    {
        Player p = new("Player2", 100);
        Player p1 = new("Player4", 300);

        playerManager.ExecuteTurn(p);
        playerManager.ExecuteTurn(p1);

        Debug.Log("Done");
    }
}
