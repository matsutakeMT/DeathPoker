using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteDatabase
{
    private Dictionary<string, Sprite> trumpCards;
    private Dictionary<string, Sprite> deathSeals;

    public SpriteDatabase()
    {
        trumpCards = Resources.LoadAll<Sprite>("TrumpCards").ToDictionary(card => card.name);
        deathSeals = Resources.LoadAll<Sprite>("DeathSeals").ToDictionary(card => card.name);
    }

    public Sprite GetTrumpCard(string name)
    {
        if (trumpCards.TryGetValue(name, out Sprite result))
            return result;
        return null;
    }

    public Sprite GetDeathSeal(string name)
    {
        if (deathSeals.TryGetValue(name, out Sprite result))
            return result;
        return null;
    }
}
