using TMPro;
using UnityEngine;

public class PlayerView
    : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private TMP_Text chipText;

    [SerializeField]
    private TMP_Text statusText;

    public void SetPlayer(
    Player player)
    {
        nameText.text =
    player.Name;

        chipText.text =
            player.Chips.ToString();

        if (player.IsDead)
        {
            statusText.text =
                "DEAD";
        }
        else if (player.IsBankrupt)
        {
            statusText.text =
                "BANKRUPT";
        }
        else
        {
            statusText.text =
                "";
        }
    }
        private void Start()
    {
        Player player =
            new Player(
                "Player1",
                1000);

        SetPlayer(player);
    }


}