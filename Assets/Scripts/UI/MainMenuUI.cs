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
    [SerializeField]
    TextMeshProUGUI mTextMeshProUGUI;

    bool mHasDisplayedStuff = false;
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
        mTextMeshProUGUI.enabled = true;
    }

    public void ProceedToNextLevel()
    {
        SceneManager.LoadScene(1);
    }
}
