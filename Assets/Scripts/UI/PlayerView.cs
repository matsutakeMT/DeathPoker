using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView
    : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text chipText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private CardView cardView1;
    [SerializeField] private CardView cardView2;
    [SerializeField] private TMP_Text betText;

    private Image bgImage;

    void Awake()
    {
        bgImage = gameObject.GetComponent<Image>();
    }

    public void SetPlayer(Player player, bool revealCards)
    {
        Debug.Log($"SetPlayer : {player.Name}");
        nameText.text = player.Name;
        chipText.text = player.Chips.ToString();

        if (player.IsDead)
        {
            bgImage.color = Color.red;
            statusText.text = "DEAD";
        }
        else if (player.IsBankrupt)
        {
            bgImage.color = Color.gray;
            statusText.text = "BANKRUPT";
        }
        else
        {
            bgImage.color = new Color(1f, 1f, 1f, 0.5f);
            statusText.text = "ALIVE";
        }

        cardView1.Clear();
        cardView2.Clear();

        if (player.Hand.Count >= 1)
        {
            cardView1.SetCard(player.Hand[0], revealCards);
        }
        if (player.Hand.Count >= 2)
        {
            cardView2.SetCard(player.Hand[1], revealCards);
        }
        betText.text = $"Bet : {player.CurrentBet}";
    }

}