using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MenuInicial, Jogando, Pausado, Upgrade, GameOver, Vitoria, Loja, ModoHorda }
    public enum Difficulty { Facil, Normal, Dificil }

    public GameState State { get; private set; } = GameState.MenuInicial;
    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;
    public int CurrentSlot { get; private set; } = 0;

    public float scoreMultiplier { get; private set; } = 1f;

    int pendingScore = -1;
    SaveData pendingData = null;
    const int startSceneIndex = 1; // Fase1_Deserto

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartGame(Difficulty difficulty, int slot)
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.Reset();
        }
        SelectDifficulty(difficulty);
        CurrentSlot = slot;

        var data = LoadGame(slot);
        if (data != null)
        {
            pendingScore = data.pontos;
            pendingData = data;
        }
        else
        {
            pendingScore = 0;
            pendingData = null;
        }

        ChangeState(GameState.Jogando);
        int scene = data != null ? data.faseAtual : startSceneIndex;
        SceneManager.LoadScene(scene);
    }

    public void ChangeState(GameState newState)
    {
        State = newState;
    }

    public void SelectDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        scoreMultiplier = DifficultyMultiplier(difficulty);
    }

    public void EnterMenuInicial(int slot = 0)
    {
        CurrentSlot = slot;
        ChangeState(GameState.MenuInicial);
        SceneManager.LoadScene("MainMenu");
        LoadGame(CurrentSlot);
    }

    public void StartGameplay()
    {
        ChangeState(GameState.Jogando);
        Time.timeScale = 1f;
        SceneManager.LoadScene(startSceneIndex);
    }

    public void PauseGame()
    {
        if (State == GameState.Pausado) return;
        ChangeState(GameState.Pausado);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (State != GameState.Pausado) return;
        ChangeState(GameState.Jogando);
        Time.timeScale = 1f;
    }

    public void EnterUpgrade()
    {
        ChangeState(GameState.Upgrade);
    }

    public void EnterGameOver()
    {
        ChangeState(GameState.GameOver);
        Time.timeScale = 1f;
        SaveGame(CurrentSlot);
        SceneManager.LoadScene("GameOver");
    }

    public void EnterVitoria()
    {
        ChangeState(GameState.Vitoria);
        Time.timeScale = 1f;
        SaveGame(CurrentSlot);
        SceneManager.LoadScene("Vitoria");
    }

    public void EnterShop()
    {
        ChangeState(GameState.Loja);
        SceneManager.LoadScene("Loja");
    }

    public void ExitShop()
    {
        SaveGame(CurrentSlot);
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            EnterMenuInicial(CurrentSlot);
        }
        else
        {
            SceneManager.LoadScene(next);
            ChangeState(GameState.Jogando);
        }
    }

    public void EnterModoHorda()
    {
        ChangeState(GameState.ModoHorda);
        SceneManager.LoadScene("TesteJogabilidade");
    }

    public void NextPhase()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            EnterMenuInicial(CurrentSlot);
        }
        else
        {
            SaveGame(CurrentSlot);
            SceneManager.LoadScene(next);
            ChangeState(GameState.Jogando);
        }
    }

    public void LoadShop()
    {
        EnterShop();
    }

    public void SaveGame(int slot)
    {
        var data = new SaveData
        {
            faseAtual = SceneManager.GetActiveScene().buildIndex,
            pontos = ScoreManager.instance ? ScoreManager.instance.pontos : 0,
            dificuldade = CurrentDifficulty
        };

        var inv = FindObjectOfType<InventoryManager>();
        if (inv != null)
        {
            data.weaponSlots = new WeaponType[inv.armas.Length];
            data.weaponAmmo = new int[inv.armas.Length];
            data.ammoCapacidade = new int[inv.armas.Length];
            for (int i = 0; i < inv.armas.Length; i++)
            {
                data.weaponSlots[i] = inv.armas[i].tipo;
                data.weaponAmmo[i] = inv.armas[i].municao;
                data.ammoCapacidade[i] = inv.armas[i].capacidade;
            }
            data.bandagens = inv.bandagens;
            data.powerUps = inv.powerUps;
        }

        var upgrade = FindObjectOfType<UpgradeSystem>();
        if (upgrade != null)
        {
            data.weaponTier = upgrade.nivelArma;
        }

        SaveSystem.Save(slot, data);
    }

    public SaveData LoadGame(int slot)
    {
        var data = SaveSystem.Load(slot);
        if (data != null)
        {
            CurrentDifficulty = data.dificuldade;
            scoreMultiplier = DifficultyMultiplier(CurrentDifficulty);
        }
        return data;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Carrega dados do slot atual se não houver pendência do StartGame
        var data = pendingData != null ? pendingData : SaveSystem.Load(CurrentSlot);

        if (ScoreManager.instance != null)
        {
            int score = pendingScore >= 0 ? pendingScore : (data != null ? data.pontos : 0);
            ScoreManager.instance.SetPontos(score);
            pendingScore = -1;
        }

        if (data != null)
        {
            var inv = FindObjectOfType<InventoryManager>();
            if (inv != null && data.weaponSlots != null)
            {
                int len = Mathf.Min(inv.armas.Length, data.weaponSlots.Length);
                for (int i = 0; i < len; i++)
                {
                    inv.armas[i].tipo = data.weaponSlots[i];
                    if (data.weaponAmmo != null && data.weaponAmmo.Length > i)
                        inv.armas[i].municao = data.weaponAmmo[i];
                    if (data.ammoCapacidade != null && data.ammoCapacidade.Length > i)
                        inv.armas[i].capacidade = data.ammoCapacidade[i];
                }
                inv.bandagens = data.bandagens;
                inv.powerUps = data.powerUps;
            }

            var upg = FindObjectOfType<UpgradeSystem>();
            if (upg != null)
                upg.nivelArma = data.weaponTier;
        }

        pendingData = null;
    }

    float DifficultyMultiplier(Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Facil: return 1f;
            case Difficulty.Normal: return 1.25f;
            case Difficulty.Dificil: return 1.5f;
        }
        return 1f;
    }
}
