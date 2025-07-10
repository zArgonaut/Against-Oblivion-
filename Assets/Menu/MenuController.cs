using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public VideoPlayer videoIntro1;
    public VideoPlayer videoIntro2;

    public GameObject painelTitulo;
    public GameObject botaoCliqueParaContinuar;
    public GameObject menuPrincipalPanel;

    void Start()
    {
        painelTitulo.SetActive(false);
        botaoCliqueParaContinuar.SetActive(false);
        menuPrincipalPanel.SetActive(false);

        videoIntro1.loopPointReached += OnIntro1Fim;
        videoIntro2.loopPointReached += OnIntro2Fim;

        videoIntro1.Play();
    }

    void OnIntro1Fim(VideoPlayer vp)
    {
        painelTitulo.SetActive(true);
        botaoCliqueParaContinuar.SetActive(true);
    }

    public void IniciarVideo2()
    {
        botaoCliqueParaContinuar.SetActive(false);
        painelTitulo.SetActive(false);
        videoIntro1.gameObject.SetActive(false);

        videoIntro2.gameObject.SetActive(true);
        videoIntro2.Play();
    }

    void OnIntro2Fim(VideoPlayer vp)
    {
        menuPrincipalPanel.SetActive(true);
    }
}
