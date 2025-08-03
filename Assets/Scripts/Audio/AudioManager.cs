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
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup SFXMixer;
    public AudioEvent GlobalDropSFX;
    public AudioEvent GlobalHurtSFX;
    public AudioEvent CodeGlitchSFX;

    private AudioSource mCodeSource;
    private AudioSource mGameSource;
    private AudioSource mUISource;
    [SerializeField]
    private AudioLevelState mAudioLevelState = AudioLevelState.Game;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.SetAudioLevelState(mAudioLevelState);
            Instance.SetGlitchLevel(mCurrentGlitchPreset);
            if (mBGM == null)
            {
                Instance.mCodeSource.Stop();
                Instance.mGameSource.Stop();
                Instance.mUISource.Stop();
            }
            else if (!Instance.mCodeSource.isPlaying)
            {
                Instance.mCodeSource.Play();
                Instance.mGameSource.Play();
                Instance.mUISource.Play();
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        StartMusic();
        SetAudioLevelState(mAudioLevelState);
        mAudioPlayer = GetComponent<AudioPlayer>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartGlitch();
    }

    void StartMusic()
    {
        mCodeSource = transform.AddComponent<AudioSource>();
        mCodeSource.resource = mBGM.mSound[0];
        mCodeSource.outputAudioMixerGroup = MusicMixer;
        mCodeSource.loop = true;

        mGameSource = transform.AddComponent<AudioSource>();
        mGameSource.resource = mBGM.mSound[1];
        mGameSource.outputAudioMixerGroup = MusicMixer;
        mGameSource.loop = true;

        mUISource = transform.AddComponent<AudioSource>();
        mUISource.resource = mBGM.mSound[2];
        mUISource.outputAudioMixerGroup = MusicMixer;
        mUISource.loop = true;

        mCodeSource.Play();
        mGameSource.Play();
        mUISource.Play();
    }

    public void SetAudioLevelState(AudioLevelState audioLevelState)
    {
        mAudioLevelState = audioLevelState;

        switch (audioLevelState)
        {
            case AudioLevelState.Code:
                mCodeSource.mute = false;
                mGameSource.mute = true;
                mUISource.mute = true;
                break;
            case AudioLevelState.Game:
                mCodeSource.mute = false;
                mGameSource.mute = false;
                mUISource.mute = false;
                break;
            case AudioLevelState.UI:
                mCodeSource.mute = true;
                mGameSource.mute = true;
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
                mGameSource.volume = volume;
                mUISource.volume = volume;

                mCodeSource.pitch = 1 + currentPreset.mPitchOffset;
                mGameSource.pitch = 1 + currentPreset.mPitchOffset;
                mUISource.pitch = 1 + currentPreset.mPitchOffset;

                float delay = Random.Range(currentPreset.mMinGlitchDelay, currentPreset.mMaxGlitchDelay);
                yield return new WaitForSecondsRealtime(delay * currentPreset.DelayModifier);
            }
            else
            {
                mCodeSource.volume = mBGM.mVolumeScale;
                mGameSource.volume = mBGM.mVolumeScale;
                mUISource.volume = mBGM.mVolumeScale;
                mCodeSource.pitch = 1f;
                mGameSource.pitch = 1f;
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
