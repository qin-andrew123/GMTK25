using TMPro;
using UnityEngine;

public class GlitchableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    void Awake()
    {
        GlitchManager.Instance.AddGlitchableObject(this);
    }
    private void OnEnable()
    {
        GlitchManager.Instance.AddGlitchableObject(this);
    }
    private void OnDisable()
    {
        GlitchManager.Instance.RemoveGlitchableObject(this);
    }
    private void OnDestroy()
    {
        GlitchManager.Instance.RemoveGlitchableObject(this);
    }
    public void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        mInteractText.enabled = bShouldbeEnabled;
    }
    public void GlitchEffect()
    {
        Debug.Log("This is a glitch effect");
    }
}
