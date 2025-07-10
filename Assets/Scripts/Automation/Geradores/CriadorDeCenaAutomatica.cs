using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CriadorDeCenaAutomatica
{
    public static void CriarCena(string nome)
    {
        var novaCena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camera = new GameObject("MainCamera");
        camera.AddComponent<Camera>();
        camera.tag = "MainCamera";

        GameObject luz = new GameObject("LuzDirecional");
        luz.AddComponent<Light>().type = LightType.Directional;

        EditorSceneManager.SaveScene(novaCena, $"Assets/Scenes/{nome}.unity");
        Debug.Log($"\uD83C\uDF04 Cena {nome} criada e salva");
    }
}
