using UnityEngine;

public class Grabbable_Glitch : GrabableObject
{
    [SerializeField]
    private float mUsageCooldown = 0.1f;
    private Vector3 DroppedPosition;
    private bool bIsSocketed = false;
    public override void UseGrabableObject(Vector3 pixelSpaceMouseInput, GameObject target)
    {
        if (gameObject != target)
        {
            return;
        }
        Debug.Log("GrabbaleGlithc");
        // place the object at the position of the code socket
        CodeSocket bestCandidate = GlobalVariables.Instance.CodeObjectManager.GetBestTarget();
        if(!bestCandidate)
        {
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mUsageCooldown);
            return;
        }
        DroppedPosition = bestCandidate.transform.position;
        bIsSocketed = true;
        DropEffect();
        // activate the socket in question
        GlobalVariables.Instance.CodeObjectManager.EnableGlitch(bestCandidate);

        GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mUsageCooldown);

    }
    public override void GrabEffect()
    {
        base.GrabEffect();
        GlobalVariables.Instance.CodeObjectManager.DisableGlitches();
       bIsSocketed = false;
    }
    public override void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        if(bIsSocketed)
        {
            mInteractText.enabled = false;
            return;
        }
        mInteractText.enabled = bShouldbeEnabled;
    }

    public override void DropEffect()
    {
        transform.SetParent(null);
        transform.position = !bIsSocketed ? GlobalVariables.Instance.PlayerRef.transform.position : DroppedPosition;
        GlobalVariables.Instance.GrabableObjectManager.AddGrabableObject(this);
        GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().GrabbedItem = null;
    }

}
