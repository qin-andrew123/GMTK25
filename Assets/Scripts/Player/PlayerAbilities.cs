using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.VFX;
using System;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField]
    private VisualEffect mDashVFX;
    public static event Action<Vector3, GameObject> OnUsedGrabbedItem;
    [SerializeField]
    private float mGlitchCooldown;
    [SerializeField]
    private float mGrabCooldown;
    private bool bHasTriggeredGlitch = false;
    private bool bHasTriggeredGrab = false;
    private bool bHasUsedItem = false;
    private GrabableObject mGrabbedItem = null;
    public GrabableObject GrabbedItem { get { return GrabbedItem; }  set { mGrabbedItem = value; } }
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
        if(Input.GetKeyDown(KeyCode.F5))
        {
            GlobalVariables.Instance.HotReloadLevel();
        }
        bool glitchInput = Input.GetButtonDown("Glitch");
        bool grabInput = Input.GetButtonDown("GrabObject");
        bool useGrabbedObject = Input.GetButtonDown("UseGrabbedObject");
        if (glitchInput && !bHasTriggeredGlitch)
        {
            bHasTriggeredGlitch = true;
            Glitch();
        }
        else if (grabInput && !bHasTriggeredGrab)
        {
            bHasTriggeredGrab = true;
            GrabItem();
        }
        else if (useGrabbedObject && !bHasUsedItem && mGrabbedItem != null)
        {
            mGrabbedItem.UseGrabableObject(Input.mousePosition, mGrabbedItem.gameObject);
        }
    }

    private void Glitch()
    {
        // Dash
        mDashVFX.Play();

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
        if (mGrabbedItem != null)
        {
            mGrabbedItem.DropEffect();
            mGrabbedItem = null;
        }
        else
        {
            GrabableObject Target = GlobalVariables.Instance.GrabableObjectManager.GetBestTarget();
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
