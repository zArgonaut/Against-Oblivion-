using UnityEngine;

public enum TipoItem
{
    Pontos,
    Vida,
    Energia
}

public class ItemPickup : MonoBehaviour
{
    public TipoItem tipo;
    public int valor = 10;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var vida = other.GetComponent<PlayerHealth>();
        var energia = other.GetComponent<PlayerEnergy>();

        switch (tipo)
        {
            case TipoItem.Vida:
                vida?.Curar(valor);
                break;
            case TipoItem.Energia:
                energia?.Restore(valor);
                break;
            case TipoItem.Pontos:
                if (ScoreManager.instance != null)
                    ScoreManager.instance.AdicionarPontos(valor);
                break;
        }

        Destroy(gameObject);
    }
}
