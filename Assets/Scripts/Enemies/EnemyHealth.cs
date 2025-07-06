using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 50;
    private int vidaAtual;

    public bool IsDead => vidaAtual <= 0;

    [Header("Feedback Visual")]
    public float tempoDestruicao = 2f;

    [Header("Pontuação")]
    public EnemyType tipo = EnemyType.Melee;

    private SpriteRenderer spriteRenderer;
    private Color corOriginal;

    void Start()
    {
        vidaAtual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            corOriginal = spriteRenderer.color;
    }

    public void LevarDano(int dano)
    {
        vidaAtual -= dano;

        if (FXManager.Instance != null)
            FXManager.Instance.Play(FXManager.FXType.Blood, transform.position);

        StartCoroutine(FlashDano());

        if (vidaAtual <= 0)
            Morrer();
    }

    System.Collections.IEnumerator FlashDano()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = corOriginal;
        }
    }

    void Morrer()
    {
        if (FXManager.Instance != null)
            FXManager.Instance.Play(FXManager.FXType.Explosion, transform.position);

        // Adiciona pontuação
        if (ScoreManager.instance != null)
            ScoreManager.instance.AdicionarPontos(tipo);

        Destroy(gameObject, tempoDestruicao);
    }
}
