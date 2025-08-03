using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    VideoPlayer mVideoPlayer;
    [SerializeField]
    Button mPlayButton;
    [SerializeField] AudioEvent mGlitchSFX;

    bool mHasDisplayedStuff = false;

    private void Start()
    {
        StartCoroutine(PlayGlitch());
    }

    IEnumerator PlayGlitch()
    {
        yield return new WaitForSeconds(2f);
        mGlitchSFX.Play2DSound();
    }

    private void Update()
    {
        if(!mVideoPlayer.isPlaying && !mHasDisplayedStuff)
        {
            EnablePlayButtons();
        }
    }

    private void EnablePlayButtons()
    {
        mHasDisplayedStuff = true;
        mPlayButton.enabled = true;
    }

    public void ProceedToNextLevel()
    {
        SceneManager.LoadScene(1);
    }
}
