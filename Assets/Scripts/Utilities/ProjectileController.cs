using UnityEngine;

public class ProjectileController : MonoBehaviour
{
public float speed = 10f;
public int dano = 10;
public float lifeTime = 3f;
public LayerMask hitMask;
[HideInInspector] public ProjectilePool pool;

    void OnEnable()
    {
        Invoke(nameof(Desativar), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.LevarDano(dano);
            }
            Desativar();
        }
    }

    void Desativar()
    {
        gameObject.SetActive(false);
        if (pool != null) pool.Devolver(this);
    }
}
