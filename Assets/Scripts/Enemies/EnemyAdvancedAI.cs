using UnityEngine;

public class EnemyAdvancedAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack, Die }

    public Transform jogador;
    public float velocidade = 2.5f;
    public float chaseDistance = 5f;
    public float attackDistance = 1.2f;
    public float distanciaStop = 2f;
    public Transform[] pontosAlternativos;

    private Rigidbody rb;
    private Animator animator;
    private EnemyPatrol patrol;
    private EnemyAttack enemyAttack;
    private EnemyHealth health;
    private Transform destino;
    private EnemyState state = EnemyState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        patrol = GetComponent<EnemyPatrol>();
        enemyAttack = GetComponent<EnemyAttack>();
        health = GetComponent<EnemyHealth>();
        destino = jogador;
        ChangeState(state);
    }

    void Update()
    {
        if (jogador == null) return;

        if (health && health.IsDead)
        {
            ChangeState(EnemyState.Die);
        }

        switch (state)
        {
            case EnemyState.Patrol:
                if (Vector3.Distance(transform.position, jogador.position) <= chaseDistance)
                    ChangeState(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                HandleChase();
                float dist = Vector3.Distance(transform.position, jogador.position);
                if (dist <= attackDistance)
                    ChangeState(EnemyState.Attack);
                else if (dist > chaseDistance)
                    ChangeState(EnemyState.Patrol);
                break;

            case EnemyState.Attack:
                FaceTarget();
                if (Vector3.Distance(transform.position, jogador.position) > attackDistance)
                    ChangeState(EnemyState.Chase);
                break;

            case EnemyState.Die:
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    void HandleChase()
    {
        float d = Vector3.Distance(transform.position, jogador.position);
        if (d < distanciaStop && pontosAlternativos.Length > 0)
        {
            int idx = Random.Range(0, pontosAlternativos.Length);
            destino = pontosAlternativos[idx];
        }
        else
        {
            destino = jogador;
        }

        Vector3 dir = (destino.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(dir.x * velocidade, rb.linearVelocity.y, dir.z * velocidade);
        if (animator) animator.SetFloat("Velocidade", Mathf.Abs(dir.x));
        transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
    }

    void FaceTarget()
    {
        Vector3 dir = (jogador.position - transform.position).normalized;
        transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);
        rb.linearVelocity = Vector3.zero;
    }

    void ChangeState(EnemyState newState)
    {
        if (state == newState) return;
        state = newState;
        if (patrol) patrol.enabled = state == EnemyState.Patrol;
        if (enemyAttack) enemyAttack.enabled = state == EnemyState.Attack;
    }
}
