using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CriaturaVoadora : MonoBehaviour
{
    [Header("Voo e Detecção")]
    public float velocidadeVoo = 4f;
    public float smoothRotationSpeed = 200f;
    public float raioDeteccao = 15f;
    public float distanciaMinimaAtaque = 3f;
    public float tempoEntreAtaques = 2.5f;

    [Header("Duração dos Ataques")]
    public float duracaoAtaque1 = 1f;
    public float duracaoAtaque2 = 1f;

    [Header("Referência ao Jogador (opcional)")]
    public Transform alvoDoJogador;

    private Animator animator;
    private float tempoProximoAtaque = 0f;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false; // Impede root motion de atrapalhar
        if (alvoDoJogador == null)
        {
            var pj = GameObject.FindGameObjectWithTag("Player");
            if (pj != null) alvoDoJogador = pj.transform;
        }
    }

    void Update()
    {
        if (alvoDoJogador == null) return;

        float dist = Vector3.Distance(transform.position, alvoDoJogador.position);
        bool detectado = dist <= raioDeteccao;

        // Animação de voo só quando detectado e não atacando
        animator.SetBool("IsFlying", detectado && !isAttacking);

        if (!detectado || isAttacking) return;

        // Garante que ela fique de frente (invertido) para o jogador
        OlharParaJogadorInvertido();

        if (dist > distanciaMinimaAtaque)
        {
            // Só se move se estiver fora do alcance de ataque
            VoarAteJogador();
        }
        else if (Time.time >= tempoProximoAtaque)
        {
            // Dentro do alcance: inicia o ataque
            tempoProximoAtaque = Time.time + tempoEntreAtaques;
            StartCoroutine(ExecutarAtaque());
        }
    }

    void VoarAteJogador()
    {
        Vector3 dir = (alvoDoJogador.position - transform.position).normalized;
        transform.position += dir * velocidadeVoo * Time.deltaTime;
    }

    void OlharParaJogadorInvertido()
    {
        Vector3 alvoPos = new Vector3(
            alvoDoJogador.position.x,
            transform.position.y,
            alvoDoJogador.position.z
        );
        Quaternion lookRot = Quaternion.LookRotation(alvoPos - transform.position);
        // Inverte 180° no eixo Y
        lookRot *= Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            lookRot,
            smoothRotationSpeed * Time.deltaTime
        );
    }

    IEnumerator ExecutarAtaque()
    {
        isAttacking = true;

        // Limpa triggers anteriores
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");

        // Escolhe ataque e duração correspondente
        bool attack1 = Random.value > 0.5f;
        string trigger = attack1 ? "Attack1" : "Attack2";
        float duration = attack1 ? duracaoAtaque1 : duracaoAtaque2;

        animator.SetTrigger(trigger);

        // Aguarda o fim da animação
        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }

    public void Morrer()
    {
        animator.SetTrigger("Die");
        foreach (var col in GetComponentsInChildren<Collider>()) col.enabled = false;
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaMinimaAtaque);
    }
}