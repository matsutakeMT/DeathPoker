using UnityEngine;
using UnityEngine.Assertions;

public class DebugSample : MonoBehaviour
{
    SpriteDatabase db;

    [SerializeField] string req = "";
    [SerializeField] Sprite ans;

    void Awake()
    {
        db = new SpriteDatabase();
    }

    void Start()
    {
        var result = db.GetDeathSeal(req);
        Assert.AreEqual(result, ans);

        Debug.Log("Done");
    }
}
