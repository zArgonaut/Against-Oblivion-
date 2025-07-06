using UnityEngine;
using System.Collections;

public class DevoradorDasAreias : MonoBehaviour
{
    public int vidaMaxima = 300;
    public GameObject[] jatosDeAreia;
    public GameObject furiaFX;
    public Transform[] pontosAtaque;
    public float intervaloAtaque = 4f;

    private int vidaAtual;
    private float timer;
    private bool emFuria = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
        timer = intervaloAtaque;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Atacar();
            timer = intervaloAtaque;
        }
    }

    void Atacar()
    {
        if (jatosDeAreia == null || jatosDeAreia.Length == 0 || pontosAtaque.Length == 0)
            return;
        foreach (var ponto in pontosAtaque)
        {
            var prefab = jatosDeAreia[Random.Range(0, jatosDeAreia.Length)];
            Instantiate(prefab, ponto.position, Quaternion.identity);
        }
    }

    public void LevarDano(int dano)
    {
        vidaAtual -= dano;
        if (!emFuria && vidaAtual <= vidaMaxima * 0.3f)
            StartCoroutine(EntrarFuria());
        if (vidaAtual <= 0)
            Morrer();
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
        Destroy(gameObject);
        if (GameManager.Instance)
            GameManager.Instance.NextPhase();
    }
}
