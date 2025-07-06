using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Game Over UI")]
    public TextMeshProUGUI finalScoreText;
    public Button retryButton;
    public Button menuButton;

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (retryButton) retryButton.onClick.AddListener(Retry);
        if (menuButton) menuButton.onClick.AddListener(BackToMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null &&
                (GameManager.Instance.State == GameManager.GameState.Jogando ||
                 GameManager.Instance.State == GameManager.GameState.Pausado))
            {
                if (GameManager.Instance.State == GameManager.GameState.Pausado)
                    Resume();
                else
                    Pause();
            }
        }

        if (GameManager.Instance != null &&
            GameManager.Instance.State == GameManager.GameState.GameOver)
        {
            if (gameOverPanel && !gameOverPanel.activeSelf)
            {
                gameOverPanel.SetActive(true);
                UpdateFinalScore();
            }
        }
    }

    public void Pause()
    {
        if (pausePanel) pausePanel.SetActive(true);
        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
        else
            Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        else
            Time.timeScale = 1f;
    }

    public void Retry()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGameplay();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.EnterMenuInicial();
        else
            SceneManager.LoadScene("MainMenu");
    }

    void UpdateFinalScore()
    {
        if (finalScoreText != null && ScoreManager.instance != null)
            finalScoreText.text = "Pontos: " + ScoreManager.instance.pontos;
    }
}
