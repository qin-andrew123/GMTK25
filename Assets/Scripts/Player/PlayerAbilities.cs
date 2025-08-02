using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.VFX;
using System;

public class PlayerAbilities : MonoBehaviour
{
    public static event Action<Vector3> OnUsedGrabbedItem;
    [SerializeField]
    private VisualEffect mGlitchVFX;
    [SerializeField]
    private float mGlitchCooldown;
    [SerializeField]
    private float mGrabCooldown;
    private bool bHasTriggeredGlitch = false;
    private bool bHasTriggeredGrab = false;
    private bool bHasUsedItem = false;
    private GrabableObject mGrabbedItem = null;

    private void OnEnable()
    {
        PlayerMovement3D.OnDashComplete += GlitchReset;
    }
    private void OnDisable()
    {
        PlayerMovement3D.OnDashComplete -= GlitchReset;
    }
    void Update()
    {
        bool glitchInput = Input.GetButtonDown("Glitch");
        bool grabInput = Input.GetButtonDown("GrabObject");
        bool useGrabbedObject = Input.GetButtonDown("UseGrabbedObject");
        if (glitchInput && !bHasTriggeredGlitch)
        {
            Debug.Log("Triggered Glitch");

            bHasTriggeredGlitch = true;
            Glitch();
        }
        else if (grabInput && !bHasTriggeredGrab)
        {
            Debug.Log("Grabbing Item");
            bHasTriggeredGrab = true;
            GrabItem();
        }
        else if (useGrabbedObject && !bHasUsedItem && mGrabbedItem)
        {
            OnUsedGrabbedItem?.Invoke(Input.mousePosition);
        }
    }

    private void Glitch()
    {
        PlayerMovement3D playerMovement3D = GlobalVariables.Instance.PlayerRef.GetComponent<PlayerMovement3D>();
        if (!playerMovement3D)
        {
            Debug.LogAssertion("Player ref does not have a playermovement3d");
            return;
        }

        playerMovement3D.IsDashing = true;
        playerMovement3D.Dash();
    }
    private void GlitchReset()
    {
        if (!bHasTriggeredGlitch)
        {
            return;
        }

        StartCoroutine(GlitchCooldown());
    }
    public void ResetUseGrabbedItem(float time)
    {
        if (!bHasUsedItem)
        {
            return;
        }

        StartCoroutine(GrabbedItemCooldown(time));
    }
    private void GrabItem()
    {
        // drop item if there is one in hand
        if (mGrabbedItem)
        {
            mGrabbedItem.DropEffect();
            mGrabbedItem = null;
        }
        else
        {
            GrabableObject Target = GrabableObjectManager.Instance.GetBestTarget();
            if (!Target)
            {
                Debug.LogWarning("PlayerAbilities.Grab(): Target Object is null.");
                bHasTriggeredGrab = false;
                return;
            }

            Target.GrabEffect();
            Target.transform.SetParent(transform, false);
            mGrabbedItem = Target;
        }
        StartCoroutine(GrabCooldown());
    }

    private IEnumerator GlitchCooldown()
    {
        yield return new WaitForSeconds(mGlitchCooldown);
        Debug.Log("End of Cooldown");
        bHasTriggeredGlitch = false;
    }

    private IEnumerator GrabCooldown()
    {
        yield return new WaitForSeconds(mGrabCooldown);
        Debug.Log("End of Cooldown");
        bHasTriggeredGrab = false;
    }
    private IEnumerator GrabbedItemCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("End of Cooldown");
        bHasUsedItem = false;
    }
}
