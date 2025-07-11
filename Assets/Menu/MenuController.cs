using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Vídeo único")]
    public VideoPlayer videoIntro;

    [Header("Imagens de fundo")]
    public GameObject imagemFundo1;
    public GameObject imagemFundo2;

    [Header("UI e transições")]
    public GameObject textoPressioneTecla;
    public Image fadeImage;

    private bool aguardandoTecla = false;
    private bool piscando = false;

    void Start()
    {
        imagemFundo1.SetActive(false);
        imagemFundo2.SetActive(false);
        textoPressioneTecla.SetActive(false);

        fadeImage.color = new Color(0, 0, 0, 1);
        FadeIn();

        videoIntro.loopPointReached += OnVideoEnd;

        StartCoroutine(ExibirImagemInicial());
    }

    IEnumerator ExibirImagemInicial()
    {
        yield return new WaitForSeconds(1f);

        imagemFundo1.SetActive(true);
        yield return new WaitForSeconds(1f);

        textoPressioneTecla.SetActive(true);
        aguardandoTecla = true;
        piscando = true;
        StartCoroutine(PiscarTexto());
    }

    void Update()
    {
        if (aguardandoTecla && Input.anyKeyDown)
        {
            aguardandoTecla = false;
            piscando = false;
            textoPressioneTecla.SetActive(false);
            StartCoroutine(IniciarVideo());
        }
    }

    IEnumerator IniciarVideo()
    {
        FadeOut();
        yield return new WaitForSeconds(1f);

        imagemFundo1.SetActive(false);
        videoIntro.gameObject.SetActive(true);
        videoIntro.Play();

        FadeIn();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(TransicaoParaMenuFinal());
    }

    IEnumerator TransicaoParaMenuFinal()
    {
        FadeOut(); 
        yield return new WaitForSeconds(1f);

        videoIntro.gameObject.SetActive(false);
        imagemFundo2.SetActive(true);

        FadeIn();
    }

    void FadeIn()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    void FadeOut()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        Color original = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }
    }

    IEnumerator PiscarTexto()
    {
        TextMeshProUGUI textoTMP = textoPressioneTecla.GetComponent<TextMeshProUGUI>();
        if (textoTMP == null)
        {
            Debug.LogWarning("TextMeshProUGUI não encontrado.");
            yield break;
        }

        while (piscando)
        {
            textoTMP.alpha = 1f;
            yield return new WaitForSeconds(0.5f);
            textoTMP.alpha = 0f;
            yield return new WaitForSeconds(0.5f);
        }

        textoTMP.alpha = 1f;
    }
}
