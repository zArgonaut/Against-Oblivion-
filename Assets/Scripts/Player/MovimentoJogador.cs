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

    [Header("Mouse Look")]
    [Tooltip("Velocidade base do mouse")]
    public float sensibilidadeMouse = 50f;
    [Tooltip("Quanto a câmera segue suavemente o mouse")]
    public float suavidadeMouse = 5f;
    [Range(-90f, 0f)] public float limiteInferior = -60f;
    [Range(0f, 90f)] public float limiteSuperior = 60f;

    private float rotYaw = 0f;
    private float rotPitch = 0f;

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

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Lock e hide do cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializa rotação atual com a rotação existente da câmera
        Vector3 angles = cameraTransform.localEulerAngles;
        rotYaw = transform.eulerAngles.y;
        rotPitch = angles.x;
    }

    void Update()
    {
        // Toggle de cursor para debug
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (!gameObject.activeInHierarchy || !controller.enabled)
            return;

        CapturarInput();
        AtualizarEstadoDoChao();
        DetectarCliqueDuplo();
        ProcessarRotacao();
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

        Vector3 frente = cameraTransform.forward;
        frente.y = 0f;
        frente.Normalize();

        Vector3 direita = cameraTransform.right;
        direita.y = 0f;
        direita.Normalize();

        direcaoMovimento = direcaoEntrada.z * frente + direcaoEntrada.x * direita;
    }

    void AtualizarEstadoDoChao()
    {
        estaNoChao = controller.isGrounded;
        if (estaNoChao && velocidadeVertical.y < 0f)
            velocidadeVertical.y = -2f;
    }

    void DetectarCliqueDuplo()
    {
        if (Input.GetKeyDown(teclaDirecional))
        {
            float agora = Time.time;
            estaCorrendo = (agora - ultimoTempoClique <= tempoMaximoEntreCliques);
            ultimoTempoClique = agora;
        }
        if (Input.GetKeyUp(teclaDirecional))
            estaCorrendo = false;
    }

    void ProcessarRotacao()
    {
        if (cameraTransform == null) return;

        // captura movimento bruto do mouse
        float deltaX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float deltaY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        // acumula rotação
        rotYaw += deltaX * Time.deltaTime;
        rotPitch -= deltaY * Time.deltaTime;
        rotPitch = Mathf.Clamp(rotPitch, limiteInferior, limiteSuperior);

        // suaviza valores antes de aplicar
        float smoothYaw  = Mathf.LerpAngle(transform.eulerAngles.y, rotYaw, Time.deltaTime * suavidadeMouse);
        float smoothPitch = Mathf.LerpAngle(cameraTransform.localEulerAngles.x, rotPitch, Time.deltaTime * suavidadeMouse);

        // aplica rotações
        transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(smoothPitch, 0f, 0f);
    }

    void ProcessarMovimento()
    {
        if (!controller.enabled) return;

        velocidadeDesejada = direcaoMovimento.magnitude > 0.1f
            ? (estaCorrendo ? velocidadeCorrer : velocidadeAndar)
            : 0f;

        float velReal = Mathf.Lerp(velocidadeAtual, velocidadeDesejada, Time.deltaTime * aceleracao);
        velocidadeAtual = velReal;

        Vector3 horiz = direcaoMovimento.normalized * velocidadeAtual;
        velocidadeVertical.y += gravidade * Time.deltaTime;

        controller.Move((horiz + velocidadeVertical) * Time.deltaTime);
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
        Debug.Log($"🧭 Vel: {velocidadeAtual:F2} | Correndo: {estaCorrendo} | NoChão: {estaNoChao}");
    }}