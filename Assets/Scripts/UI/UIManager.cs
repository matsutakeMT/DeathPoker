using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform leftPlayerContainer;
    [SerializeField] private Transform rightPlayerContainer;
    [SerializeField] private PlayerView playerViewPrefab;
    [SerializeField] private Transform selfArea;
    [SerializeField] private Transform communityArea;
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private TMP_Text death1Text;
    [SerializeField] private TMP_Text death3Text;
    [SerializeField] private TMP_Text death5Text;
    [SerializeField] private DeathPanel deathPanel;
    [SerializeField] private WinnerPanel winnerPanel;
    [SerializeField] private TMP_Text potText;

    private List<CardView> communityViews = new();
    private List<PlayerView> opponentViews = new();

    private PlayerView selfView;

    public void Initialize()
    {
        CreateCommunityViews();
        CreateSelfView();
        CreatePlayerViews();
        RefreshPlayers();
        RefreshCommunity();
        RefreshDeathCounts();
    }

    private void CreatePlayerViews()
    {
        for (int i = 1; i < gameManager.Players.Count; i++)
        {
            PlayerView view = Instantiate(playerViewPrefab);

            Transform parent = i % 2 == 0 ? leftPlayerContainer : rightPlayerContainer;

            view.transform.SetParent(parent, false);

            opponentViews.Add(view);
        }
    }

    private void CreateSelfView()
    {
        selfView = Instantiate(playerViewPrefab);
        selfView.transform.SetParent(selfArea, false);
    }

    private void CreateCommunityViews()
    {
        for (int i = 0; i < 5; i++)
        {
            CardView view = Instantiate(cardViewPrefab);

            view.transform.SetParent(communityArea, false);
            view.SetBack();

            communityViews.Add(view);
        }
    }

    public void RefreshPlayers()
    {
        selfView.SetPlayer(gameManager.Players[0], true);

        int viewIndex = 0;

        for (int i = 1; i < gameManager.Players.Count; i++)
        {
            opponentViews[viewIndex].SetPlayer(gameManager.Players[i], false);
            viewIndex++;
        }
    }

    public void RefreshCommunity()
    {
        for (int i = 0; i < communityViews.Count; i++)
        {
            if (i < gameManager.CommunityCards.Count)
            {
                communityViews[i].SetCard(gameManager.CommunityCards[i], true);
            }
            else
            {
                communityViews[i].SetBack();
            }
        }
    }

    public void RefreshDeathCounts()
    {
        Player player = gameManager.Players[0];

        death1Text.text = $"Death1 : {player.Death1Count}";
        death3Text.text = $"Death3 : {player.Death3Count}";
        death5Text.text = $"Death5 : {player.Death5Count}";
    }

    public void ShowDeath(Card causeCard)
    {
        deathPanel.Show(causeCard);
    }
    public void HideDeath()
    {
        deathPanel.Hide();
    }

    public void ShowdownReveal()
    {
        int viewIndex = 0;

        for (int i = 1; i < gameManager.Players.Count; i++)
        {
            Player player = gameManager.Players[i];

            bool reveal = !player.IsDead;

            opponentViews[viewIndex].SetPlayer(player, reveal);

            viewIndex++;
        }
    }

    public void ShowWinner(string winner, string hand)
    {
        winnerPanel.Show(winner, hand);
    }

    public void HideWinner()
    {
        winnerPanel.Hide();
    }

    public void RefreshPot(int pot)
    {
        potText.text = $"Pot : {pot}";
    }

}