using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
    [SerializeField] private Image causeCardImage;
    [SerializeField] private Image causeSealImage;

    public void Show(Card card)
    {
        gameObject.SetActive(true);

        causeCardImage.sprite = SpriteDatabase.Instance.GetTrumpCard(card.SpriteName);
        causeSealImage.sprite = SpriteDatabase.Instance.GetDeathSeal(card.Seal.SpriteName);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}

