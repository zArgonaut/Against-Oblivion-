using UnityEngine;

public static class GeradorDeInimigos
{
    public static GameObject CriarInimigo(string nome, bool comArma)
    {
        GameObject inimigo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        inimigo.name = nome;
        inimigo.tag = "Enemy";
        inimigo.transform.localScale = new Vector3(1, 2, 1);
        inimigo.AddComponent<Rigidbody>();
        inimigo.AddComponent<IAInimigoBasica>();

        if (comArma)
        {
            GameObject arma = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            arma.name = "Arma";
            arma.transform.SetParent(inimigo.transform);
            arma.transform.localPosition = new Vector3(0.5f, 1, 0);
        }

        return inimigo;
    }
}
