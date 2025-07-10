using UnityEngine;

public class IAInimigoBasica : MonoBehaviour
{
    private Transform alvo;

    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
            alvo = player.transform;
    }

    void Update()
    {
        if (alvo == null) return;

        float dist = Vector3.Distance(transform.position, alvo.position);
        if (dist < 10f)
        {
            transform.position = Vector3.MoveTowards(transform.position, alvo.position, Time.deltaTime * 2f);
        }
    }
}
