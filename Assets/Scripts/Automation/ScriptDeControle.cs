using UnityEngine;

public class ScriptDeControle : MonoBehaviour
{
    public float velocidade = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direcao = new Vector3(h, 0, v).normalized;
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }
}
