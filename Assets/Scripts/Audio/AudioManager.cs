using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField, Tooltip("Music track")]
    private AudioEvent mBGM;
    private AudioPlayer mAudioPlayer;
    [SerializeField, Tooltip("Glitch presets for each level")]
    private AudioGlitchPreset[] mGlitchPresets;
    [SerializeField]
    private int mCurrentGlitchPreset = 0;

    private AudioSource mBGMSource;

    private void Awake()
    {
        Instance = this;
        mAudioPlayer = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        mBGMSource = transform.AddComponent<AudioSource>();
        mBGMSource.resource = mBGM.mSound[0];
        mBGMSource.volume = mBGM.mVolumeScale;
        mBGMSource.loop = true;
        mBGMSource.Play();

        StartGlitch();
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
                mBGMSource.volume = volume;

                mBGMSource.pitch = 1 + currentPreset.mPitchOffset;

                float delay = Random.Range(currentPreset.mMinGlitchDelay, currentPreset.mMaxGlitchDelay);
                yield return new WaitForSecondsRealtime(delay * currentPreset.DelayModifier);
            }
            else
            {
                mBGMSource.volume = mBGM.mVolumeScale;
                mBGMSource.pitch = 1f;
                yield return null;
            }
        }
    }

    public AudioSource PlayAudioEvent(AudioEvent audioEvent)
    {
        return mAudioPlayer.PlayAudioEvent(audioEvent);
    }
}
