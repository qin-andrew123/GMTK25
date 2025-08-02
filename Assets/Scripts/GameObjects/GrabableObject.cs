using System.Collections;
using TMPro;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField]
    private bool bGlitchAudio = false;

    void Awake()
    {

    }
    private void OnEnable()
    {
        StartCoroutine(AddSelfToManager());
    }
    private void OnDisable()
    {
        GrabableObjectManager.Instance.RemoveGrabableObject(this);
    }
    private void OnDestroy()
    {
        GrabableObjectManager.Instance.RemoveGrabableObject(this);
    }
    public void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        mInteractText.enabled = bShouldbeEnabled;
    }
    public void GrabEffect()
    {
        Debug.Log("this is a grab effect");
        transform.localPosition = Vector3.zero;
        ChangeInteractTextStatus(false);
        GrabableObjectManager.Instance.RemoveGrabableObject(this);
        if (bGlitchAudio)
        {
            AudioManager.Instance.SetGlitchLevel(1);
        }
    }

    public void DropEffect()
    {
        Debug.Log("Drop");
        transform.SetParent(null);
        GrabableObjectManager.Instance.AddGrabableObject(this);
    }

    private IEnumerator AddSelfToManager()
    {
        yield return new WaitUntil(() => GrabableObjectManager.Instance != null);
        GrabableObjectManager.Instance.AddGrabableObject(this);
    }
}
