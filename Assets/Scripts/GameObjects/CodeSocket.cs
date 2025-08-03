using TMPro;
using UnityEngine;

public enum GlitchEffectType
{
    CHANGE_OBJECTS,
    CHANGE_SOUND,
    CHANGE_PHYSICS
}
public class CodeSocket : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField]
    private GlitchEffectType mEffectType;
    public GlitchEffectType EffectType { get { return mEffectType; } }
    private bool bHasBeenGlitched = false;
    public bool HasBeenGlitched { set { bHasBeenGlitched = value; } }
    void Awake()
    {
        GlobalVariables.Instance.CodeObjectManager.AddGlitchableObject(this);
    }
    private void OnDestroy()
    {
        GlobalVariables.Instance.CodeObjectManager.RemoveGlitchableObject(this);
    }
    public void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        if (bHasBeenGlitched)
        {
            mInteractText.text = "GLITCHED";
            mInteractText.enabled = true;
            return;
        }
        else
        {
            mInteractText.text = "Glitch";
        }
        mInteractText.enabled = bShouldbeEnabled;
    }

}
