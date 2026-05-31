using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager GameManager;

    [SerializeField]
    private Transform leftPlayerContainer;

    [SerializeField]
    private Transform rightPlayerContainer;

    [SerializeField]
    private PlayerView playerViewPrefab;

    private void Start()
    {
        CreatePlayerViews();
    }

    private void CreatePlayerViews()
    {
        for (int i = 1;
            i < GameManager.Players.Count;
            i++)
        {
            PlayerView view = Instantiate(playerViewPrefab);

            Transform parent = i % 2 == 0 ? leftPlayerContainer : rightPlayerContainer;

            view.transform.SetParent(parent, false);

            view.SetPlayer(GameManager.Players[i]);
        }
    }


}