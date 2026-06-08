using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private string GameSceneName;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        Debug.Log("starting game...");
        startButton.onClick.RemoveListener(StartGame);
        SceneManager.LoadScene(GameSceneName);
    }
}
