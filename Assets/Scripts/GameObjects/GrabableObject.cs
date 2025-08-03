using System.Collections;
using TMPro;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    public virtual void UseGrabableObject(Vector3 pixelSpaceMouseInput, GameObject target) { }
    [SerializeField]
    protected TextMeshPro mInteractText;
    [SerializeField]
    private bool bGlitchAudio = false;
    [SerializeField]
    private AudioEvent mGrabSFX;

    private void OnEnable()
    {
        StartCoroutine(AddSelfToManager());
    }
    private void OnDisable()
    {
    }
    private void OnDestroy()
    {
        GlobalVariables.Instance.GrabableObjectManager.RemoveGrabableObject(this);
    }
    public virtual void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        mInteractText.enabled = bShouldbeEnabled;
    }
    public virtual void GrabEffect()
    {
        Debug.Log("this is a grab effect");
        transform.localPosition = Vector3.zero;
        ChangeInteractTextStatus(false);
        GlobalVariables.Instance.GrabableObjectManager.RemoveGrabableObject(this);
        if (bGlitchAudio)
        {
            AudioManager.Instance.SetGlitchLevel(1);
        }
        mGrabSFX?.Play2DSound();
    }

    public virtual void DropEffect()
    {
        Debug.Log("Drop");
        transform.SetParent(null);
        GlobalVariables.Instance.GrabableObjectManager.AddGrabableObject(this);
    }

    private IEnumerator AddSelfToManager()
    {
        yield return new WaitUntil(() => GlobalVariables.Instance.GrabableObjectManager != null);
        GlobalVariables.Instance.GrabableObjectManager.AddGrabableObject(this);
    }
}
