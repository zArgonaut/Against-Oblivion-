using UnityEngine;

public class HordaManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveConfig
    {
        public GameObject[] inimigos;
    }

    [Header("Configura\u00e7\u00e3o de Spawn")]
    public WaveConfig[] waves;
    public Transform[] pontosSpawn;
    public int inimigosBase = 5;
    public int incrementoInimigos = 2;
    public float intervaloBase = 5f;
    public float reducaoIntervalo = 0.5f;
    public float intervaloMinimo = 1f;

    [Header("Boss")]
    public GameObject bossPrefab;
    public int pontosParaBoss = 100;

    private float timer;
    private int waveAtual = 0;
    private int inimigosRestantes = 0;
    private float intervaloAtual;
    private bool bossSpawnado = false;

    void Start()
    {
        IniciarWave(0);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (!bossSpawnado && ScoreManager.instance != null && ScoreManager.instance.pontos >= GetPontosParaBoss())
        {
            SpawnBoss();
            return;
        }

        if (inimigosRestantes > 0 && timer <= 0f)
        {
            SpawnInimigo();
            inimigosRestantes--;
            timer = intervaloAtual;
        }
        else if (inimigosRestantes <= 0 && !ExistemInimigosVivos())
        {
            IniciarWave(waveAtual + 1);
        }
    }

    void SpawnInimigo()
    {
        if (pontosSpawn.Length == 0 || waves.Length == 0) return;

        int idxWave = Mathf.Min(waveAtual, waves.Length - 1);
        var lista = waves[idxWave].inimigos;
        if (lista == null || lista.Length == 0) return;

        int idxInimigo = Random.Range(0, lista.Length);
        int idxPonto = Random.Range(0, pontosSpawn.Length);

        Instantiate(lista[idxInimigo], pontosSpawn[idxPonto].position, Quaternion.identity);
    }

    void SpawnBoss()
    {
        if (bossPrefab == null || pontosSpawn.Length == 0) return;
        bossSpawnado = true;
        int idxPonto = Random.Range(0, pontosSpawn.Length);
        Instantiate(bossPrefab, pontosSpawn[idxPonto].position, Quaternion.identity);
    }

    public int GetPontosParaBoss()
    {
        var diff = GameManager.Instance ? GameManager.Instance.CurrentDifficulty : GameManager.Difficulty.Normal;
        switch (diff)
        {
            case GameManager.Difficulty.Facil:
                return Mathf.RoundToInt(pontosParaBoss * 0.75f);
            case GameManager.Difficulty.Dificil:
                return Mathf.RoundToInt(pontosParaBoss * 1.5f);
            default:
                return pontosParaBoss;
        }
    }

    void IniciarWave(int index)
    {
        waveAtual = index;
        intervaloAtual = Mathf.Max(intervaloBase - reducaoIntervalo * waveAtual, intervaloMinimo);
        inimigosRestantes = inimigosBase + incrementoInimigos * waveAtual;
        timer = intervaloAtual;
    }

    bool ExistemInimigosVivos()
    {
        return FindObjectsOfType<EnemyHealth>().Length > 0;
    }
}
