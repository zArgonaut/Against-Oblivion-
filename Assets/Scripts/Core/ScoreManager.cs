using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum EnemyType
{
    Melee,
    Ranged,
    Flyer
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [System.Serializable]
    public struct EnemyScore
    {
        public EnemyType tipo;
        public int valorBase;
    }

    public EnemyScore[] valores;
    private Dictionary<EnemyType, int> lookup = new Dictionary<EnemyType, int>();

    [HideInInspector] public int pontos = 0;
    [HideInInspector] public int pontosGastos = 0;
    public TextMeshProUGUI textoPontos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var e in valores)
                lookup[e.tipo] = e.valorBase;
        }
        else Destroy(gameObject);
    }

    public void AdicionarPontos(EnemyType tipo)
    {
        int valor = lookup.ContainsKey(tipo) ? lookup[tipo] : 1;
        AdicionarPontos(valor);
    }

    public void AdicionarPontos(int valor)
    {
        float mult = GameManager.Instance ? GameManager.Instance.scoreMultiplier : 1f;
        pontos += Mathf.RoundToInt(valor * mult);
        AtualizarUI();
    }

    public void SetPontos(int valor)
    {
        pontos = valor;
        AtualizarUI();
    }

    public void Reset()
    {
        SetPontos(0);
        pontosGastos = 0;
    }

    public bool SpendPoints(int valor)
    {
        if (pontos < valor) return false;
        pontos -= valor;
        pontosGastos += valor;
        AtualizarUI();
        return true;
    }

    void AtualizarUI()
    {
        if (textoPontos) textoPontos.text = "PONTOS: " + pontos;
    }
}
