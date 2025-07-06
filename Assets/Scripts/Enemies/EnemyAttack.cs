using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int dano = 10;
    public float intervalo = 1.2f;
    public float raio = 0.6f;
    public Transform pontoAtaque;
    public LayerMask playerLayer;

    private float timer = 0f;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Collider[] hits = Physics.OverlapSphere(pontoAtaque.position, raio, playerLayer);
            if (hits.Length > 0)
            {
                hits[0].GetComponent<PlayerHealth>()?.LevarDano(dano);
                timer = intervalo;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pontoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pontoAtaque.position, raio);
        }
    }
}
