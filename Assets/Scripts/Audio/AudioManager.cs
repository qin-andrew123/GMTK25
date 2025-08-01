using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField, Tooltip("Music track")]
    private AudioEvent mBGM;
    private AudioPlayer mAudioPlayer;

    private void Awake()
    {
        Instance = this;
        mAudioPlayer = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        PlayAudioEvent(mBGM);
    }

    public void PlayAudioEvent(AudioEvent audioEvent)
    {
        mAudioPlayer.PlayAudioEvent(audioEvent);
    }
}
