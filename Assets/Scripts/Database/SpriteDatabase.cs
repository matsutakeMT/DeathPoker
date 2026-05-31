using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteDatabase
{
    public static SpriteDatabase Instance { get; } = new SpriteDatabase();

    private Dictionary<string, Sprite> trumpCards;
    private Dictionary<string, Sprite> deathSeals;

    private SpriteDatabase()
    {
        trumpCards = Resources.LoadAll<Sprite>("TrumpCards").ToDictionary(card => card.name);

        deathSeals = Resources.LoadAll<Sprite>("DeathSeals").ToDictionary(card => card.name);
    }

    public Sprite GetTrumpCard(string name)
    {
        trumpCards.TryGetValue(name, out Sprite result);

        return result;
    }

    public Sprite GetDeathSeal(string name)
    {
        deathSeals.TryGetValue(name, out Sprite result);

        return result;
    }
}