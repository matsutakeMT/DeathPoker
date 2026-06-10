using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DeathPanel : MonoBehaviour
{
    [SerializeField] private Image causeCardImage;
    [SerializeField] private Image causeSealImage;
    [SerializeField] private Image backgroundImage;

    public void Show(Card card)
    {
        Color c = Color.red;
        c.a = backgroundImage.color.a;
        backgroundImage.color = c;

        gameObject.SetActive(true);

        causeCardImage.sprite = SpriteDatabase.Instance.GetTrumpCard(card.SpriteName);
        causeSealImage.sprite = SpriteDatabase.Instance.GetDeathSeal(card.Seal.SpriteName);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator ColorizeBackground()
    {
        float endTime = Time.time + 2f;
        while (Time.time < endTime)
        {
            Color c = Color.HSVToRGB(Time.time / 1.325f % 1, 1, 1);
            c.a = backgroundImage.color.a;
            backgroundImage.color = c;
            yield return null;
        }

    yield break;
    }
}

