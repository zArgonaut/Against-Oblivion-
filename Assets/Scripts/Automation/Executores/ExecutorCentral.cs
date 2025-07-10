using UnityEditor;
using UnityEngine;

public static class ExecutorCentral
{
    [MenuItem("Automacao/Executar Tudo")]
    public static void ExecutarTudo()
    {
        Debug.Log("\uD83E\uDD16 Execucao automatizada iniciada");
        CriadorDeCenaAutomatica.CriarCena("Cena_Gerada");
        GeradorDePersonagem.CriarPersonagemBase("Heroi");
        GeradorDeInimigos.CriarInimigo("Drone", true);
        Debug.Log("\u2705 Tudo executado com sucesso");
    }

    [MenuItem("Automacao/Criar Prefab Jogador")]
    public static void CriarPrefabJogador()
    {
        var jogador = GeradorDePersonagem.CriarPersonagemBase("JogadorPrefab");
        PrefabUtility.SaveAsPrefabAsset(jogador, "Assets/Prefabs/Jogador.prefab");
        Object.DestroyImmediate(jogador);
        Debug.Log("\u2705 Prefab salvo");
    }
}
