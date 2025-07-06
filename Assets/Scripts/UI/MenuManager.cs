using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Jogar(int slot)
    {
        GameManager.Instance.StartGame(GameManager.Difficulty.Normal, slot);
    }

    public void MostrarLoja()
    {
        SceneManager.LoadScene("Loja");
    }

    public void Salvar(int slot)
    {
        GameManager.Instance.SaveGame(slot);
    }

    public void Carregar(int slot)
    {
        GameManager.Instance.StartGame(GameManager.Difficulty.Normal, slot);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
