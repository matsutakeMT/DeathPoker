using TMPro;
using UnityEngine;

public class WinnerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerText;

    [SerializeField] private TMP_Text handText;

    public void Show(string winner, string hand)
    {
        gameObject.SetActive(true);

        winnerText.text = winner;

        handText.text = hand;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}