using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("Audio that plays on start")]
    private AudioEvent[] mStartupSounds;
    private List<AudioSource> mSources = new();

    private void Awake()
    {
        AudioSource audioSource = transform.AddComponent<AudioSource>();
        mSources.Add(audioSource);

        foreach (AudioEvent audioEvent in mStartupSounds)
        {
            PlayAudioEvent(audioEvent);
        }
    }

    public int PlayAudioEvent(AudioEvent audioEvent)
    {
        if (audioEvent == null || audioEvent.mSound == null) { return -1; }

        if (audioEvent.bIsSpatial)
        {
            AudioSource audioSource = transform.AddComponent<AudioSource>();
            mSources.Add(audioSource);
            audioSource.resource = audioEvent.mSound;
            audioSource.spatialBlend = audioEvent.mSpatialBlend;
            audioSource.volume = audioEvent.mVolumeScale;
            audioSource.loop = audioEvent.bIsLoop;
            audioSource.Play();

            return mSources.Count - 1;
        }
        else if (audioEvent.bIsLoop)
        {
            return FindAndPlaySource(audioEvent);
        }
        else
        {
            if (audioEvent.mSound is AudioClip)
            {
                mSources[0].PlayOneShot(audioEvent.mSound as AudioClip, audioEvent.mVolumeScale);
                return -1;
            }
            else
            {
                // Random containers cannot be played as a fire-and-forget SFX for some stupid reason
                return FindAndPlaySource(audioEvent);
            }
        }
    }

    // Searches for an available audio source to play from
    // If there are none, creates a new one
    private int FindAndPlaySource(AudioEvent audioEvent)
    {
        int availSource = -1;
        for (int i = 0; i < mSources.Count; ++i)
        {
            if (!mSources[i].isPlaying || mSources[i].clip == null)
            {
                availSource = i;
                break;
            }
        }

        if (availSource == -1)
        {
            AudioSource audioSource = transform.AddComponent<AudioSource>();
            mSources.Add(audioSource);
            audioSource.resource = audioEvent.mSound;
            audioSource.volume = audioEvent.mVolumeScale;
            audioSource.loop = audioEvent.bIsLoop;
            audioSource.Play();
            return mSources.Count - 1;
        }
        else
        {
            mSources[availSource].resource = audioEvent.mSound;
            mSources[availSource].volume = audioEvent.mVolumeScale;
            mSources[availSource].loop = audioEvent.bIsLoop;
            mSources[availSource].Play();
            return availSource;
        }
    }
}
