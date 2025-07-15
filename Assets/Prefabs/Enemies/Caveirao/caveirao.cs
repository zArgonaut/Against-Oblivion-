using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CaveiraoAI : MonoBehaviour
{
    [Header("Parâmetros de Comportamento")]
    public float velocidadeAndar = 2f;
    public float velocidadeCorrer = 5f;
    public float raioDeteccao = 18f;
    public float raioAtaque = 8f;
    public float tempoEntreAtaques = 2f;
    public float velocidadeRotacao = 10f;
    public float gravidade = -9.81f;

    [Header("Sistema de Patrulha Simples")]
    [Tooltip("Arraste aqui um objeto vazio da cena que servirá como o destino da patrulha.")]
    public Transform pontoDestinoPatrulha;

    [Header("Referências")]
    public Transform alvoDoJogador;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocidadeVertical;
    private float proximoAtaqueDisponivel = 0f;

    private Vector3 posicaoInicial;
    private bool indoParaDestino = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        posicaoInicial = transform.position;

        if (alvoDoJogador == null)
        {
            var pj = GameObject.FindGameObjectWithTag("Player");
            if (pj != null) alvoDoJogador = pj.transform;
            else
            {
                Debug.LogError("CaveirãoAI: Player não encontrado. Desabilitando AI.");
                enabled = false;
                return;
            }
        }
    }

    void Update()
    {
        if (alvoDoJogador == null) return;

        if (controller.isGrounded && velocidadeVertical.y < 0f)
            velocidadeVertical.y = -2f;
        
        velocidadeVertical.y += gravidade * Time.deltaTime;

        float distancia = Vector3.Distance(transform.position, alvoDoJogador.position);

        if (distancia > raioDeteccao)
        {
            Patrulhar();
        }
        else if (distancia > raioAtaque)
        {
            Perseguir();
        }
        else
        {
            Atacar();
        }
    }

    private void Patrulhar()
    {
        if (pontoDestinoPatrulha == null)
        {
            animator.SetFloat("velocidade", 0f);
            controller.Move(velocidadeVertical * Time.deltaTime);
            return;
        }

        Vector3 alvoAtual = indoParaDestino ? pontoDestinoPatrulha.position : posicaoInicial;

        if (Vector3.Distance(transform.position, alvoAtual) < 1.5f)
        {
            indoParaDestino = !indoParaDestino;
        }
        
        Vector3 direcaoParaPonto = (alvoAtual - transform.position).normalized;
        Vector3 movimentoHorizontal = direcaoParaPonto * velocidadeAndar;

        controller.Move((movimentoHorizontal + velocidadeVertical) * Time.deltaTime);
        animator.SetFloat("velocidade", velocidadeAndar);
        OlharParaOAlvo(alvoAtual);
    }

    private void Perseguir()
    {
        animator.SetFloat("velocidade", velocidadeCorrer);
        OlharParaOAlvo(alvoDoJogador.position);
        Vector3 direcao = transform.forward;
        Vector3 movimentoHorizontal = direcao * velocidadeCorrer;
        controller.Move((movimentoHorizontal + velocidadeVertical) * Time.deltaTime);
    }

    private void Atacar()
    {
        animator.SetFloat("velocidade", 0f);
        OlharParaOAlvo(alvoDoJogador.position);
        if (Time.time >= proximoAtaqueDisponivel)
        {
            animator.SetTrigger("attack3");
            proximoAtaqueDisponivel = Time.time + tempoEntreAtaques;
        }
        controller.Move(velocidadeVertical * Time.deltaTime);
    }

    private void OlharParaOAlvo(Vector3 alvoPosicao)
    {
        Vector3 direcao = (alvoPosicao - transform.position).normalized;
        direcao.y = 0;
        if (direcao.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * velocidadeRotacao);
        }
    }

    public void Morrer()
    {
        animator.SetTrigger("Die");
        enabled = false;
        if(controller != null) controller.enabled = false;
    }
}