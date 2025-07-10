using UnityEngine;
using UnityEngine.Video;

public class MenuController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject menuOpcoes;

    void Start()
    {
        if (menuOpcoes != null)
            menuOpcoes.SetActive(false);

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        if (videoPlayer != null && videoPlayer.isPlaying && Input.anyKeyDown)
        {
            videoPlayer.Stop();
            ShowOptions();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        ShowOptions();
    }

    void ShowOptions()
    {
        if (menuOpcoes != null)
            menuOpcoes.SetActive(true);
    }
}
