using UnityEngine;
using System.Collections;

public class SombraCongelada : MonoBehaviour
{
    public int vidaMaxima = 250;
    public GameObject projecaoPrefab;
    public GameObject furiaFX;
    public Transform[] pontosProjecao;
    public float intervaloAtaque = 3f;

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
            LancaProjecao();
            timer = intervaloAtaque;
        }
    }

    void LancaProjecao()
    {
        if (projecaoPrefab == null || pontosProjecao.Length == 0)
            return;
        foreach (var p in pontosProjecao)
            Instantiate(projecaoPrefab, p.position, Quaternion.identity);
    }

    public void LevarDano(int dano)
    {
        if (emFuria) dano = Mathf.CeilToInt(dano * 0.75f);
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
        intervaloAtaque = Mathf.Max(0.8f, intervaloAtaque / 2f);
        yield return null;
    }

    void Morrer()
    {
        Destroy(gameObject);
        if (GameManager.Instance)
            GameManager.Instance.NextPhase();
    }
}
