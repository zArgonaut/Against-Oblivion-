using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovimentoJogador : MonoBehaviour
{
    [Header("Parâmetros de Movimento")]
    public float velocidadeAndar = 3f;
    public float velocidadeCorrer = 6f;
    public float suavidadeRotacao = 10f;
    public float alturaPulo = 1.5f;
    public float gravidade = -9.81f;
    public float aceleracao = 10f;

    [Header("Duplo Clique para Correr")]
    public float tempoMaximoEntreCliques = 0.3f;
    private float ultimoTempoClique = -1f;
    private KeyCode teclaDirecional = KeyCode.W;

    [Header("Referências")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocidadeVertical;
    private float velocidadeAtual = 0f;
    private float velocidadeDesejada = 0f;
    private bool estaNoChao;
    private bool estaCorrendo;

    private Vector3 direcaoEntrada;
    private Vector3 direcaoMovimento;

    void Awake()
{
    controller = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();

    if (controller == null)
    {
        Debug.LogError("CharacterController está ausente!");
    }

    if (cameraTransform == null && Camera.main != null)
        cameraTransform = Camera.main.transform;
}

    void Update()
    {
        if (!gameObject.activeInHierarchy || !controller.enabled)
            return;

        CapturarInput();
        AtualizarEstadoDoChao();
        DetectarCliqueDuplo();
        ProcessarMovimento();
        ProcessarPulo();
        AtualizarAnimacoes();
        ExibirDebug();
    }

    void CapturarInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direcaoEntrada = new Vector3(horizontal, 0f, vertical).normalized;

        if (cameraTransform == null) return;

        Vector3 frenteCamera = cameraTransform.forward;
        frenteCamera.y = 0f;
        frenteCamera.Normalize();

        Vector3 direitaCamera = cameraTransform.right;
        direitaCamera.y = 0f;
        direitaCamera.Normalize();

        direcaoMovimento = direcaoEntrada.z * frenteCamera + direcaoEntrada.x * direitaCamera;
    }

    void AtualizarEstadoDoChao()
    {
        estaNoChao = controller.isGrounded;
        if (estaNoChao && velocidadeVertical.y < 0)
            velocidadeVertical.y = -2f;
    }

    void DetectarCliqueDuplo()
    {
        if (Input.GetKeyDown(teclaDirecional))
        {
            float tempoAgora = Time.time;
            estaCorrendo = (tempoAgora - ultimoTempoClique <= tempoMaximoEntreCliques);
            ultimoTempoClique = tempoAgora;
        }

        if (Input.GetKeyUp(teclaDirecional))
        {
            estaCorrendo = false;
        }
    }

    void ProcessarMovimento()
    {
    if (!controller.enabled || !gameObject.activeInHierarchy) return;

    velocidadeDesejada = direcaoMovimento.magnitude > 0.1f
        ? (estaCorrendo ? velocidadeCorrer : velocidadeAndar)
        : 0f;

    // Suavização da velocidade
    float velocidadeReal = Mathf.Lerp(velocidadeAtual, velocidadeDesejada, Time.deltaTime * aceleracao);
    velocidadeAtual = velocidadeReal;

    if (direcaoMovimento.magnitude >= 0.1f)
    {
        float anguloAlvo = Mathf.Atan2(direcaoMovimento.x, direcaoMovimento.z) * Mathf.Rad2Deg;
        float angulo = Mathf.LerpAngle(transform.eulerAngles.y, anguloAlvo, Time.deltaTime * suavidadeRotacao);
        transform.rotation = Quaternion.Euler(0f, angulo, 0f);
    }

    Vector3 movimentoHorizontal = direcaoMovimento.normalized * velocidadeAtual;
    velocidadeVertical.y += gravidade * Time.deltaTime;

    Vector3 movimentoTotal = movimentoHorizontal + velocidadeVertical;
    controller.Move(movimentoTotal * Time.deltaTime);
    }

    void ProcessarPulo()
    {
        if (Input.GetButtonDown("Jump") && estaNoChao)
        {
            velocidadeVertical.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
            animator.ResetTrigger("Pulou");
            animator.SetTrigger("Pulou");
        }
    }

    void AtualizarAnimacoes()
    {
        animator.SetFloat("Velocidade", velocidadeAtual / velocidadeCorrer, 0.1f, Time.deltaTime);
        animator.SetBool("Correndo", estaCorrendo);
        animator.SetBool("NoChao", estaNoChao);
    }

    void ExibirDebug()
    {
        Debug.Log($"🧭 Velocidade: {velocidadeAtual:F2} | Correndo: {estaCorrendo} | NoChao: {estaNoChao}");
    }
}