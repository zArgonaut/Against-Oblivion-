using UnityEngine;
using System.Collections;

// ProgressoFase controla o deslocamento da fase e a transição para o chefe
// baseado na pontuação atual obtida pelo jogador.
public class ProgressoFase : MonoBehaviour
{
    public Transform player;
    public Transform pontoTeleportBoss;
    public BossTrigger bossTrigger;
    public float velocidade = 2f;
    public bool moverJogador = true;

    bool bossAtivado = false;

    void Update()
    {
        if (bossAtivado) return;

        if (moverJogador && player != null)
            player.Translate(Vector3.forward * velocidade * Time.deltaTime);
        else
            transform.Translate(Vector3.back * velocidade * Time.deltaTime);

        if (ScoreManager.instance != null && ScoreManager.instance.pontos >= PontuacaoParaBoss())
            StartCoroutine(AtivarBoss());
    }

    int PontuacaoParaBoss()
    {
        if (GameManager.Instance == null)
            return 100;
        switch (GameManager.Instance.CurrentDifficulty)
        {
            case GameManager.Difficulty.Facil: return 75;
            case GameManager.Difficulty.Dificil: return 150;
            default: return 100;
        }
    }

    IEnumerator AtivarBoss()
    {
        bossAtivado = true;
        yield return new WaitForSeconds(0.5f); // pequeno delay/efeito

        if (player != null && pontoTeleportBoss != null)
            player.position = pontoTeleportBoss.position;

        if (bossTrigger != null)
            bossTrigger.gameObject.SetActive(true);
    }

    // Método auxiliar para testes forçando a pontuação do boss
    public void ForcarBoss()
    {
        StartCoroutine(AtivarBoss());
    }
}
