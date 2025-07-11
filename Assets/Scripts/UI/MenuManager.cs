using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Painel de Carregamento")]
    public GameObject painelCarregando;
    public TextMeshProUGUI textoCarregando;

    [Header("Botões de Menu")]
    public Button[] botoesMenu;

    [Header("Configuração da Fase")]
    public string nomeCenaFase1 = "Fase1";

    private bool carregando = false;

    void Start()
    {
        if (painelCarregando != null)
            painelCarregando.SetActive(false);

        if (textoCarregando != null)
            textoCarregando.gameObject.SetActive(false);
    }

    public void NovoJogo()
    {
        if (carregando) return;

        carregando = true;

        foreach (Button btn in botoesMenu)
        {
            if (btn != null)
                btn.interactable = false;
        }

        if (painelCarregando != null)
            painelCarregando.SetActive(true);

        if (textoCarregando != null)
            textoCarregando.gameObject.SetActive(true);

        StartCoroutine(ExecutarCarregamento());
    }

    IEnumerator ExecutarCarregamento()
    {
        float tempoMinimo = 1.5f;
        float tempoDecorrido = 0f;
        bool visivel = true;

        while (tempoDecorrido < tempoMinimo)
        {
            if (textoCarregando != null)
                textoCarregando.alpha = visivel ? 1f : 0f;

            visivel = !visivel;
            tempoDecorrido += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        if (textoCarregando != null)
            textoCarregando.alpha = 1f;

        SceneManager.LoadScene(nomeCenaFase1);
    }

    public void Continuar()
    {
        Debug.Log("TODO: Abrir tela de saves ou continuar.");
    }

    public void MostrarOpcoes()
    {
        Debug.Log("TODO: Abrir tela de opções.");
    }

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }
}
