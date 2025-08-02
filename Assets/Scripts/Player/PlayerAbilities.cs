using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.VFX;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField]
    private VisualEffect mGlitchVFX;
    [SerializeField]
    private float mGlitchCooldown;
    [SerializeField]
    private float mGrabCooldown;
    private bool bHasTriggeredGlitch = false;
    private bool bHasTriggeredGrab = false;
    private GrabableObject mGrabbedItem = null;

    private void OnEnable()
    {
        PlayerMovement3D.OnDashComplete += GlitchReset;
    }
    private void OnDisable()
    {
        PlayerMovement3D.OnDashComplete -= GlitchReset;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float glitchInput = Input.GetAxisRaw("Ability1");
        float grabInput = Input.GetAxisRaw("Ability2");

        if ((glitchInput >= 1.0f) && !bHasTriggeredGlitch)
        {
            Debug.Log("Triggered Glitch");

            bHasTriggeredGlitch = true;
            Glitch();
        }
        else if ((grabInput >= 1.0f) && !bHasTriggeredGrab)
        {
            Debug.Log("Grabbing Item");
            bHasTriggeredGrab = true;
            GrabItem();
        }
    }

    private void Glitch()
    {
        // Dash

        PlayerMovement3D playerMovement3D = GlobalVariables.Instance.PlayerRef.GetComponent<PlayerMovement3D>();
        if (!playerMovement3D)
        {
            Debug.LogAssertion("Player ref does not have a playermovement3d");
            return;
        }

        playerMovement3D.IsDashing = true;
        playerMovement3D.Dash();
        //GlitchableObject Target = GlobalVariables.Instance.GlitchManager.GetBestTarget();
        //if (!Target)
        //{
        //    Debug.LogWarning("PlayerAbilities.Glitch(): Target Object is null.");
        //    bHasTriggeredGlitch = false;
        //    return;
        //}
        //Target.GlitchEffect();
    }
    private void GlitchReset()
    {
        if (!bHasTriggeredGlitch)
        {
            return;
        }

        StartCoroutine(GlitchCooldown());
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

}
