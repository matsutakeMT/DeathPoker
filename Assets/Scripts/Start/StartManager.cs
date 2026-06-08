using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] private string GameSceneName;
    
    [SerializeField] private Button startButton;
    [SerializeField] private PlayerCount playerCount;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        Debug.Log($"starting game with {playerCount.Count} players.");
        startButton.onClick.RemoveListener(StartGame);
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene(GameSceneName);
    }

    private void SceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.playerCount = playerCount.Count;

        SceneManager.sceneLoaded -= SceneLoaded;
    }
}
