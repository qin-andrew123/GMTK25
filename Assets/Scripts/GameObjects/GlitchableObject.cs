using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class GlitchableObject : MonoBehaviour
{
    [SerializeField]
    private VisualEffect mGlitchVFX;
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField]
    private Transform mLocationToGlitchTo;
    [SerializeField]
    private float mDelayBeforeTransport = 0.2f;
    [SerializeField]
    private AudioLevelState mAudioLevelState;
    [SerializeField]
    private AudioEvent mTeleportSFX;

    private Animator mPlayerAnimator;
    void Awake()
    {
        GlobalVariables.Instance.GlitchManager.AddGlitchableObject(this);
    }
    private void OnDestroy()
    {
        GlobalVariables.Instance.GlitchManager.RemoveGlitchableObject(this);
    }
    public void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        mInteractText.enabled = bShouldbeEnabled;
    }
    public void GlitchEffect()
    {
        mPlayerAnimator = GlobalVariables.Instance.PlayerRef.GetComponent<Animator>();
        mPlayerAnimator.SetTrigger("Glitch");

        mGlitchVFX.Play();
        Debug.Log("This is a glitch effect");

        StartCoroutine(GlitchTransportDelay());

    }

    private IEnumerator GlitchTransportDelay()
    {
        yield return new WaitForSeconds(mDelayBeforeTransport);

        GlobalVariables.Instance.LevelManager.MovePlayerToPosition(mLocationToGlitchTo);
        AudioManager.Instance.SetAudioLevelState(mAudioLevelState);
        mTeleportSFX.Play2DSound();

        PlayerMovement3D playerMovement = GlobalVariables.Instance.PlayerRef.gameObject.GetComponent<PlayerMovement3D>();
        if (!playerMovement)
        {
            Debug.LogWarning("PlayerMovement3d is null on playerref");
            yield break;
        }
        playerMovement.IsDashing = false;

    }
}
