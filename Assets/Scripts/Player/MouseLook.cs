using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensibilidade do Mouse")]
    public float sensibilidadeMouse = 100f;

    [Header("Limites de Rotação Vertical da Câmera")]
    [Range(-90f, 0f)] // Ajuste para a câmera não ir muito para cima ou para baixo
    public float limiteInferior = -60f;
    [Range(0f, 90f)]
    public float limiteSuperior = 60f;

    [Header("Referência do Jogador")]
    public Transform corpoJogador; // A transformação do seu jogador (o objeto com o CharacterController)

    private float rotacaoX = 0f; // Rotação vertical da câmera
    private float rotacaoY = 0f; // Rotação horizontal do jogador

    void Start()
    {
        // Trava e esconde o cursor do mouse no centro da tela
        Cursor.lockState = CursorLockMode.Locked;

        // Se o corpoJogador não foi atribuído no Inspector, tenta encontrar o pai da câmera
        if (corpoJogador == null)
        {
            if (transform.parent != null)
            {
                corpoJogador = transform.parent;
                Debug.LogWarning("Corpo do Jogador não atribuído. Usando o pai da câmera como corpo do jogador.");
            }
            else
            {
                Debug.LogError("Corpo do Jogador não atribuído e a câmera não tem pai. A rotação horizontal pode não funcionar corretamente.");
            }
        }
    }

    void Update()
    {
        // Captura o input do mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        // Atualiza a rotação vertical da câmera
        rotacaoX -= mouseY;
        rotacaoX = Mathf.Clamp(rotacaoX, limiteInferior, limiteSuperior); // Limita a rotação vertical

        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);

        // Atualiza a rotação horizontal do jogador
        if (corpoJogador != null)
        {
            corpoJogador.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // Se não houver corpoJogador, rotaciona a própria câmera horizontalmente
            // Isso pode causar rotação indesejada do CharacterController se ele não estiver sendo controlado pela rotação do corpoJogador
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}