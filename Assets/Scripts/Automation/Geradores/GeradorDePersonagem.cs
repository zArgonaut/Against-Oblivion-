using UnityEngine;

public static class GeradorDePersonagem
{
    public static GameObject CriarPersonagemBase(string nome)
    {
        GameObject personagem = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        personagem.name = nome;
        personagem.AddComponent<Rigidbody>();
        personagem.AddComponent<CharacterController>();
        personagem.AddComponent<ScriptDeControle>();
        return personagem;
    }
}
