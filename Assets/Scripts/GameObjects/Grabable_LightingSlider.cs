using UnityEngine;

public class Grabable_LightingSlider : GrabableObject
{
    [SerializeField]
    private float mGrabbedItemUsedCooldown;
    [SerializeField]
    private float mUseRadius;
    public override void UseGrabableObject(Vector3 pixelSpaceMouseInput)
    {
        GameObject mDarknessVolume = GlobalVariables.Instance.DarknessVolume;
        if (!mDarknessVolume)
        {
            Debug.LogWarning("Darkness Volume does not exist in current level. Is this correct?");
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mGrabbedItemUsedCooldown);
            return;
        }

        GameObject player = GlobalVariables.Instance.PlayerRef;
        if (!player)
        {
            Debug.LogAssertion("Player Ref is null.");
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mGrabbedItemUsedCooldown);
            return;
        }

        float mCurrentDistance = Vector3.Distance(player.transform.position, mDarknessVolume.transform.position);

        if (mCurrentDistance <= mUseRadius)
        {
            bool bIsActive = mDarknessVolume.activeInHierarchy;
            mDarknessVolume.SetActive(!bIsActive);
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mGrabbedItemUsedCooldown);
        }
    }
}
