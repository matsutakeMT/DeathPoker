using TMPro;
using UnityEngine;

public class PlayerView
    : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text chipText;

    [SerializeField] private TMP_Text statusText;

    [SerializeField] private CardView cardView1;

    [SerializeField] private CardView cardView2;

    public void SetPlayer(Player player, bool revealCards)
    {
        Debug.Log($"SetPlayer : {player.Name}");
        nameText.text = player.Name;

        chipText.text = player.Chips.ToString();

        if (player.IsDead)
        {
            statusText.text = "DEAD";
        }
        else if (player.IsBankrupt)
        {
            statusText.text = "BANKRUPT";
        }
        else
        {
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
    }

}