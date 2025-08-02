using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public enum AudioLevelState
{
    Code,
    Game,
    UI
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField]
    private AudioEvent mBGM;
    private AudioPlayer mAudioPlayer;
    [SerializeField, Tooltip("Glitch presets for each level")]
    private AudioGlitchPreset[] mGlitchPresets;
    [SerializeField]
    private int mCurrentGlitchPreset = 0;

    private AudioSource mCodeSource;
    private AudioSource mUISource;
    private AudioLevelState mAudioLevelState = AudioLevelState.Game;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mAudioPlayer = GetComponent<AudioPlayer>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        mCodeSource = transform.AddComponent<AudioSource>();
        mCodeSource.resource = mBGM.mSound[0];
        mCodeSource.loop = true;

        mUISource = transform.AddComponent<AudioSource>();
        mUISource.resource = mBGM.mSound[1];
        mUISource.loop = true;

        mCodeSource.Play();
        mUISource.Play();

        StartGlitch();
    }

    public void SetAudioLevelState(AudioLevelState audioLevelState)
    {
        mAudioLevelState = audioLevelState;

        switch (audioLevelState)
        {
            case AudioLevelState.Code:
                mCodeSource.mute = false;
                mUISource.mute = true;
                break;
            case AudioLevelState.Game:
                mCodeSource.mute = false;
                mUISource.mute = false;
                break;
            case AudioLevelState.UI:
                mCodeSource.mute = true;
                mUISource.mute = false;
                break;
        }
    }

    public void SetGlitchLevel(int level)
    {
        if (level > mCurrentGlitchPreset)
        {
            mCurrentGlitchPreset = level;
        }
    }

    public void StartGlitch()
    {
        StartCoroutine(GlitchBGM());
    }

    IEnumerator GlitchBGM()
    {
        while (true) 
        {
            AudioGlitchPreset currentPreset = mGlitchPresets[mCurrentGlitchPreset];

            if (currentPreset.bDoGlitch)
            {
                float volume = Random.Range(0f, 1f / currentPreset.mGlitchIntensity);
                volume = Mathf.Round(volume);
                volume *= currentPreset.mGlitchIntensity;
                mCodeSource.volume = volume;
                mUISource.volume = volume;

                mCodeSource.pitch = 1 + currentPreset.mPitchOffset;
                mUISource.pitch = 1 + currentPreset.mPitchOffset;

                float delay = Random.Range(currentPreset.mMinGlitchDelay, currentPreset.mMaxGlitchDelay);
                yield return new WaitForSecondsRealtime(delay * currentPreset.DelayModifier);
            }
            else
            {
                mCodeSource.volume = mBGM.mVolumeScale;
                mUISource.volume = mBGM.mVolumeScale;
                mCodeSource.pitch = 1f;
                mUISource.pitch = 1f;
                yield return null;
            }
        }
    }

    public AudioSource PlayAudioEvent(AudioEvent audioEvent)
    {
        return mAudioPlayer.PlayAudioEvent(audioEvent);
    }
}
