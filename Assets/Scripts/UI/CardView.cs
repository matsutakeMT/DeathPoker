using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{

    [SerializeField] private Image cardImage;

    [SerializeField] private Image sealImage;


    public void SetCard(Card card, bool visible)
    {

        Debug.Log($"SetCard : {card.SpriteName}");

        gameObject.SetActive(true);

        if (!visible)
        {
            cardImage.sprite = SpriteDatabase.Instance.GetTrumpCard("TrumpBack");

            sealImage.enabled = false;

            return;
        }

        cardImage.sprite = SpriteDatabase.Instance.GetTrumpCard(card.SpriteName);

        if (card.Seal != null)
        {
            sealImage.enabled = true;

            sealImage.sprite = SpriteDatabase.Instance.GetDeathSeal(card.Seal.SpriteName);
        }
        else
        {
            sealImage.enabled = false;
        }
    }



    public void Clear()
    {
        gameObject.SetActive(false);
    }

    public void SetBack()
    {
        cardImage.sprite = SpriteDatabase.Instance.GetTrumpCard("TrumpBack");

        sealImage.enabled = false;
    }

}