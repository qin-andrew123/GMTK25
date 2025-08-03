using UnityEngine;

public class Grabbable_Glitch : GrabableObject
{
    [SerializeField]
    private float mUsageCooldown = 0.1f;
    private Vector3 DroppedPosition;
    public override void UseGrabableObject(Vector3 pixelSpaceMouseInput) 
    {
        Debug.Log("GrabbaleGlithc");
        // place the object at the position of the code socket
        CodeSocket bestCandidate = GlobalVariables.Instance.CodeObjectManager.GetBestTarget();
        DroppedPosition = bestCandidate.transform.position;
        // activate the socket in question
        GlobalVariables.Instance.CodeObjectManager.EnableGlitch(bestCandidate);

        GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mUsageCooldown);

    }
    public override void GrabEffect()
    {
        base.GrabEffect();
        GlobalVariables.Instance.CodeObjectManager.DisableGlitches();
    }
    public override void DropEffect()
    {
        transform.SetParent(null);
        transform.position = DroppedPosition;
        GlobalVariables.Instance.GrabableObjectManager.AddGrabableObject(this);
        GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().GrabbedItem = null;
    }

}
