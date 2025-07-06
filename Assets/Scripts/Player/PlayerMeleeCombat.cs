using UnityEngine;

public class PlayerMeleeCombat : MonoBehaviour
{
    public int dano = 20;
    public float alcance = 1f;
    public LayerMask inimigoLayer;
    public Transform pontoAtaque;

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
            Atacar();
    }

    void Atacar()
    {
        Collider[] hits = Physics.OverlapSphere(pontoAtaque.position, alcance, inimigoLayer);
        if (hits.Length > 0)
        {
            EnemyHealth inimigo = hits[0].GetComponent<EnemyHealth>();
            if (inimigo) inimigo.LevarDano(dano);
        }
    }
}
