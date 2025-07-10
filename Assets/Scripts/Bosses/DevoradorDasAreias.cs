using UnityEngine;
using System.Collections;

public class DevoradorDasAreias : MonoBehaviour
{
    public int vidaMaxima = 300;
    public GameObject[] jatosDeAreia;
    public GameObject furiaFX;
    public Transform[] pontosAtaque;
    public float intervaloAtaque = 4f;

    public float raioDeteccao = 15f;
    public float distanciaMinimaAtaque = 5f;
    public float distanciaAtaque1 = 2f;
    public float velocidadeMovimento = 3f;
    public Transform alvoDoJogador;

    private int vidaAtual;
    private float timer;
    private bool emFuria = false;
    private bool jogadorDetectado = false;
    private bool emDistanciaDeAtaque = false;
    private bool emDistanciaAtaque1 = false;

    private Animator animator;

    void Start()
    {
        vidaAtual = vidaMaxima;
        timer = intervaloAtaque;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Componente Animator não encontrado em DevoradorDasAreias. Por favor, adicione um Animator a este GameObject.");
        }

        GameObject playerObject = GameObject.Find("Meshy_Merged_Animations (1)");
        if (playerObject != null)
        {
            alvoDoJogador = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Objeto do jogador chamado 'Meshy_Merged_Animations (1)' não encontrado. O boss não seguirá ou detectará o jogador.");
        }
    }

    void Update()
    {
        if (alvoDoJogador == null)
        {
            if (animator != null)
            {
                animator.SetBool("IsChasing", false);
            }
            return;
        }

        float distanciaAoJogador = Vector3.Distance(transform.position, alvoDoJogador.position);
        bool novoJogadorDetectado = distanciaAoJogador <= raioDeteccao;
        bool novaEmDistanciaDeAtaque = distanciaAoJogador <= distanciaMinimaAtaque;
        bool novaEmDistanciaAtaque1 = distanciaAoJogador <= distanciaAtaque1;

        // --- DEBUG: Log das distâncias e flags ---
        Debug.Log($"Distância ao jogador: {distanciaAoJogador:F2}");
        Debug.Log($"Jogador Detectado (raioDeteccao): {novoJogadorDetectado}");
        Debug.Log($"Em Distância de Ataque (distanciaMinimaAtaque): {novaEmDistanciaDeAtaque}");
        Debug.Log($"Em Distância Ataque 1 (distanciaAtaque1): {novaEmDistanciaAtaque1}");


        // Atualiza o parâmetro IsChasing do Animator
        if (animator != null)
        {
            bool deveriaEstarCorrendo = novoJogadorDetectado && !novaEmDistanciaDeAtaque;

            if (deveriaEstarCorrendo != animator.GetBool("IsChasing"))
            {
                animator.SetBool("IsChasing", deveriaEstarCorrendo);
                Debug.Log($"SETANDO IsChasing para: {deveriaEstarCorrendo}");
            }
        }

        jogadorDetectado = novoJogadorDetectado;
        emDistanciaDeAtaque = novaEmDistanciaDeAtaque;
        emDistanciaAtaque1 = novaEmDistanciaAtaque1;


        if (jogadorDetectado)
        {
            if (!emDistanciaDeAtaque)
            {
                MoverParaOJogador();
            }
            else
            {
                OlharParaOJogador();
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                AtacarComDistancia();
                timer = intervaloAtaque; // Reinicia o timer após o ataque
                Debug.Log($"Timer resetado para {intervaloAtaque}");
            }
        }
        else
        {
             if (animator != null)
             {
                animator.SetBool("IsChasing", false);
             }
        }
    }

    void MoverParaOJogador()
    {
        Vector3 direcaoAoJogador = (alvoDoJogador.position - transform.position).normalized;
        transform.position += direcaoAoJogador * velocidadeMovimento * Time.deltaTime;
        OlharParaOJogador();
    }

    void OlharParaOJogador()
    {
        transform.LookAt(new Vector3(alvoDoJogador.position.x, transform.position.y, alvoDoJogador.position.z));
    }

    void AtacarComDistancia()
    {
        if (!jogadorDetectado)
        {
            Debug.Log("Tentou atacar mas jogador não detectado.");
            return;
        }

        if (emDistanciaAtaque1)
        {
            if (animator != null)
            {
                animator.SetTrigger("attack3");
                Debug.Log("ACIONANDO TRIGGER: attack3");
            }
            ExecutarLogicaAtaque(jatosDeAreia[0]);
        }
        else if (emDistanciaDeAtaque)
        {
            if (animator != null)
            {
                animator.SetTrigger("attack1");
                Debug.Log("ACIONANDO TRIGGER: attack1");
            }
            ExecutarLogicaAtaque(jatosDeAreia[1]);
        }
        else
        {
            Debug.Log("Jogador detectado mas fora dos ranges de ataque definidos (distanciaMinimaAtaque ou distanciaAtaque1).");
        }
    }

    void ExecutarLogicaAtaque(GameObject prefabDeJato)
    {
        if (jatosDeAreia == null || jatosDeAreia.Length == 0 || pontosAtaque.Length == 0)
        {
            Debug.LogWarning("JatosDeAreia ou PontosAtaque não configurados no Inspector.");
            return;
        }

        // Garante que o prefabDeJato existe antes de instanciar
        if (prefabDeJato == null)
        {
            Debug.LogWarning("Prefab de Jato de Areia nulo. Verifique a configuração do array jatosDeAreia no Inspector.");
            return;
        }

        foreach (var ponto in pontosAtaque)
        {
            if (ponto != null)
            {
                Instantiate(prefabDeJato, ponto.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Ponto de ataque nulo. Verifique a configuração do array pontosAtaque no Inspector.");
            }
        }
        Debug.Log($"Instanciando {prefabDeJato.name} nos pontos de ataque.");
    }

    public void LevarDano(int dano)
    {
        vidaAtual -= dano;
        Debug.Log($"Boss levou {dano} de dano. Vida Atual: {vidaAtual}");
        if (!emFuria && vidaAtual <= vidaMaxima * 0.3f)
        {
            StartCoroutine(EntrarFuria());
            Debug.Log("Boss entrou em Fúria!");
        }
        if (vidaAtual <= 0)
        {
            Morrer();
            Debug.Log("Boss morreu!");
        }
    }

    IEnumerator EntrarFuria()
    {
        emFuria = true;
        if (furiaFX) Instantiate(furiaFX, transform.position, Quaternion.identity);
        intervaloAtaque = Mathf.Max(1f, intervaloAtaque / 2f);
        yield return null;
    }

    void Morrer()
    {
        if (animator != null)
        {
            animator.SetTrigger("death");
            Debug.Log("ACIONANDO TRIGGER: death");
        }

        StartCoroutine(DestroyAfterAnimation(3f));

        if (GameManager.Instance)
            GameManager.Instance.NextPhase();
    }

    IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMinimaAtaque);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque1);
    }
}