using System.Collections;
using TMPro;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField]
    private bool bGlitchAudio = false;
    private void OnEnable()
    {
        StartCoroutine(AddSelfToManager());
        PlayerAbilities.OnUsedGrabbedItem += UseGrabableObject;
    }
    private void OnDisable()
    {
        PlayerAbilities.OnUsedGrabbedItem -= UseGrabableObject;
    }
    private void OnDestroy()
    {
        GrabableObjectManager.Instance.RemoveGrabableObject(this);
        PlayerAbilities.OnUsedGrabbedItem -= UseGrabableObject;
    }
    protected virtual void UseGrabableObject(Vector3 pixelSpaceMouseInput) { }
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
