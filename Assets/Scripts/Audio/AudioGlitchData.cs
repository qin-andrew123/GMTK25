using UnityEngine;

[CreateAssetMenu(fileName = "AudioGlitchPreset", menuName = "Scriptable Objects/AudioGlitchPreset")]
public class AudioGlitchPreset : ScriptableObject
{
    public bool bDoGlitch = false;
    [Tooltip("On a scale of 0.1-1, how extreme should glitches be in volume")]
    public float mGlitchIntensity = 0.25f;
    public float mPitchOffset = -0.1f;
    [Tooltip("Minimum amount of seconds to wait between glitches")]
    public float mMinGlitchDelay = 0.1f;
    [Tooltip("Maximum amount of seconds to wait between glitches")]
    public float mMaxGlitchDelay = 0.5f;
    [Tooltip("Smaller numbers means glitches are faster")]
    public float DelayModifier = 0.8f;
}
