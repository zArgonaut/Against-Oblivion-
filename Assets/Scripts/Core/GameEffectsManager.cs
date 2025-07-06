using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEffectsManager : MonoBehaviour
{
    public static GameEffectsManager Instance { get; private set; }

    [Header("UI - Pausa")]
    public GameObject menuPausa;

    [Header("Efeitos Visuais de Tela")]
    public Image flashBranco;
    public float duracaoFlash = 0.2f;

    private bool pausado = false;
    private Vector3 posOriginalCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        if (menuPausa != null) menuPausa.SetActive(false);
        if (flashBranco != null) flashBranco.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            AlternarPausa();
    }

    public void AlternarPausa()
    {
        pausado = !pausado;
        if (GameManager.Instance != null)
        {
            if (pausado)
                GameManager.Instance.PauseGame();
            else
                GameManager.Instance.ResumeGame();
        }
        else
        {
            Time.timeScale = pausado ? 0 : 1;
        }
        if (menuPausa != null) menuPausa.SetActive(pausado);
        Debug.Log("Jogo " + (pausado ? "Pausado" : "Despausado"));
    }

    public void TremorCamera(Camera cam, float intensidade = 0.2f, float duracao = 0.15f)
    {
        StartCoroutine(Tremor(cam, intensidade, duracao));
    }

    private IEnumerator Tremor(Camera cam, float intensidade, float duracao)
    {
        if (cam == null) yield break;

        posOriginalCamera = cam.transform.localPosition;
        float tempo = 0f;

        while (tempo < duracao)
        {
            Vector2 deslocamentoAleatorio = Random.insideUnitCircle * intensidade;
            cam.transform.localPosition = posOriginalCamera + (Vector3)deslocamentoAleatorio;
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.transform.localPosition = posOriginalCamera;
    }

    public void FlashBranco()
    {
        if (flashBranco != null)
        {
            StopCoroutine("Flash");
            StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        flashBranco.color = new Color(1, 1, 1, 1);
        float tempo = 0;

        while (tempo < duracaoFlash)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, tempo / duracaoFlash);
            flashBranco.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        flashBranco.color = new Color(1, 1, 1, 0);
    }
}
