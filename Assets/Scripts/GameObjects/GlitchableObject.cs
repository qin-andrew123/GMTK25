using System.Collections;
using TMPro;
using UnityEngine;

public class GlitchableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField]
    private Transform mLocationToGlitchTo;
    [SerializeField]
    private AudioLevelState mAudioLevelState;
    [SerializeField]
    private AudioEvent mTeleportSFX;
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
        Debug.Log("This is a glitch effect");
        GlobalVariables.Instance.LevelManager.MovePlayerToPosition(mLocationToGlitchTo);
        AudioManager.Instance.SetAudioLevelState(mAudioLevelState);
        mTeleportSFX.Play2DSound();

        PlayerMovement3D playerMovement = GlobalVariables.Instance.PlayerRef.gameObject.GetComponent<PlayerMovement3D>();
        if (!playerMovement)
        {
            Debug.LogWarning("PlayerMovement3d is null on playerref");
            return;
        }
        playerMovement.IsDashing = false;
    }
}
