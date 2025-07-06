using UnityEngine;
using System.Collections;

public class ColossoInstavel : MonoBehaviour
{
    public int vidaMaxima = 400;
    public GameObject pedraPrefab;
    public GameObject ondaPrefab;
    public GameObject furiaFX;
    public Transform[] pontosLancamento;
    public float intervaloAtaque = 5f;

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
            RealizarAtaque();
            timer = intervaloAtaque;
        }
    }

    void RealizarAtaque()
    {
        int tipo = Random.Range(0, 2);
        if (tipo == 0)
        {
            if (pedraPrefab == null || pontosLancamento.Length == 0)
                return;
            foreach (var t in pontosLancamento)
                Instantiate(pedraPrefab, t.position, Quaternion.identity);
            if (emFuria && ondaPrefab != null)
                Instantiate(ondaPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            if (ondaPrefab != null)
                Instantiate(ondaPrefab, transform.position, Quaternion.identity);
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
        if (furiaFX)
            Instantiate(furiaFX, transform.position, Quaternion.identity);
        intervaloAtaque = Mathf.Max(1.5f, intervaloAtaque / 1.5f);
        yield return null;
    }

    void Morrer()
    {
        Destroy(gameObject);
        if (GameManager.Instance)
            GameManager.Instance.NextPhase();
    }
}
