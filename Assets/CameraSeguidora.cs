using UnityEngine;

public class CameraSeguidora : MonoBehaviour
{
    public Transform alvo;
    public Vector3 offset = new Vector3(0, 1.8f, -4f);
    public float suavidade = 5f;
    public float sensibilidadeMouse = 2f;

    private float rotacaoY = 0f;

    void LateUpdate()
    {
        if (alvo == null) return;

        rotacaoY += Input.GetAxis("Mouse X") * sensibilidadeMouse;

        Quaternion rotacao = Quaternion.Euler(0, rotacaoY, 0);
        Vector3 posDesejada = alvo.position + rotacao * offset;

        transform.position = Vector3.Lerp(transform.position, posDesejada, Time.deltaTime * suavidade);
        transform.LookAt(alvo.position + Vector3.up * 1.5f);
    }
}