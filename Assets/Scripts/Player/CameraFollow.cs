
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform alvo;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float suavidade = 0.125f;

    void LateUpdate() {
        if (alvo == null) return;
        Vector3 posDesejada = alvo.position + offset;
        Vector3 posSuavizada = Vector3.Lerp(transform.position, posDesejada, suavidade * Time.deltaTime);
        transform.position = posSuavizada;
    }
}
