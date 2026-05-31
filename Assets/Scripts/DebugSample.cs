using UnityEngine;
using UnityEngine.Assertions;

public class DebugSample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player p = new Player("Player1", 1000);

        Assert.AreEqual(p.Name, "Player1");

        p.Bet(100);

        Assert.AreEqual(p.Chips, 900);
        Assert.AreEqual(p.CurrentBet, 100);

        p.ResetRound();

        Assert.AreEqual(p.Chips, 900);
        Assert.AreEqual(p.CurrentBet, 0);

        Debug.Log("Test passed!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
