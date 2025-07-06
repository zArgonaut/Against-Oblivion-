using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float velocidade = 2f;
    public Transform[] pontosPatrulha;
    private int indice = 0;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (pontosPatrulha.Length == 0) return;
        Transform alvo = pontosPatrulha[indice];
        Vector3 dir = (alvo.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(dir.x * velocidade, rb.linearVelocity.y, dir.z * velocidade);
        if (Vector3.Distance(transform.position, alvo.position) < 0.2f)
            indice = (indice + 1) % pontosPatrulha.Length;
    }
}
