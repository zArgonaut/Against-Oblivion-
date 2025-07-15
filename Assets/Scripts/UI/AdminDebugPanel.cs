using UnityEngine;
using UnityEngine.UI;

public class AdminDebugPanel : MonoBehaviour
{
    public Canvas panel;
    public KeyCode toggleKey = KeyCode.F10;
    public Button spawnBossButton;
    public Button addScoreButton;
    public Button subScoreButton;
    public Dropdown difficultyDropdown;

    void Awake()
    {
        if (panel != null)
            panel.enabled = Debug.isDebugBuild;
        if (spawnBossButton != null)
            spawnBossButton.onClick.AddListener(SpawnBoss);
        if (addScoreButton != null)
            addScoreButton.onClick.AddListener(() => ModifyScore(100));
        if (subScoreButton != null)
            subScoreButton.onClick.AddListener(() => ModifyScore(-100));
        if (difficultyDropdown != null)
            difficultyDropdown.onValueChanged.AddListener(ChangeDifficulty);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (panel != null)
                panel.enabled = !panel.enabled;
        }
    }

    void SpawnBoss()
    {
        var horda = FindObjectOfType<HordaManager>();
        if (horda == null || horda.bossPrefab == null || horda.pontosSpawn.Length == 0)
            return;
        int idx = Random.Range(0, horda.pontosSpawn.Length);
        Instantiate(horda.bossPrefab, horda.pontosSpawn[idx].position, Quaternion.identity);
    }

    void ModifyScore(int value)
    {
        if (ScoreManager.instance != null)
            ScoreManager.instance.AdicionarPontos(value);
    }

    void ChangeDifficulty(int index)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SelectDifficulty((GameManager.Difficulty)index);
    }
}
