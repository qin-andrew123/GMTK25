using UnityEngine;

public class Grabbable_VolumeSlider : GrabableObject
{
    [SerializeField]
    private GameObject mBulletPrefab;
    [SerializeField]
    private Transform mFirePoint;
    [SerializeField]
    private float mGrabbedItemUsedCooldown;

    protected override void UseGrabableObject(Vector3 pixelSpaceMouseInput)
    {
        Vector3 bulletShootDirection = Vector3.right;
        Vector3 startingPosition = mFirePoint.transform.position;
        startingPosition = Camera.main.WorldToScreenPoint(startingPosition);

        bulletShootDirection = pixelSpaceMouseInput - startingPosition;
        bulletShootDirection.Normalize();

        Debug.Log("BulletShootDirection: " + bulletShootDirection);

        if (!mBulletPrefab)
        {
            Debug.LogAssertion("There is no prefab for the volumeslider bullet");
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mGrabbedItemUsedCooldown);
            return;
        }

        GameObject go = Instantiate(mBulletPrefab, mFirePoint.position, Quaternion.identity);
        AudioBullet bullet = go.GetComponent<AudioBullet>();
        if (!bullet)
        {
            Debug.LogAssertion("AudioBullet prefab does not have a audiobullet script");
            GlobalVariables.Instance.PlayerRef.GetComponent<PlayerAbilities>().ResetUseGrabbedItem(mGrabbedItemUsedCooldown);
            return;
        }

        bullet.MoveDirection = bulletShootDirection;
    }


}
