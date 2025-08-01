using System.Collections;
using TMPro;
using UnityEngine;

public class GlitchableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    void Awake()
    {

    }
    private void OnEnable()
    {
        StartCoroutine(AddSelfToManager());
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

    private IEnumerator AddSelfToManager()
    {
        yield return new WaitUntil(() => GlitchManager.Instance != null);
        GlitchManager.Instance.AddGlitchableObject(this);
    }
}
