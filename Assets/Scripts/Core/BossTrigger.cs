using UnityEngine;

// BossTrigger ativa a luta do chefe quando o jogador entra no colisor.
// Ele instancia o prefab do chefe e notifica o GameManager.
public class BossTrigger : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnPoint;
    bool ativado = false;

    void OnTriggerEnter(Collider other)
    {
        if (ativado || bossPrefab == null) return;
        if (!other.CompareTag("Player")) return;

        Instantiate(bossPrefab, spawnPoint ? spawnPoint.position : transform.position, Quaternion.identity);
        ativado = true;
        if (GameManager.Instance)
            GameManager.Instance.ChangeState(GameManager.GameState.Jogando);
    }
}
