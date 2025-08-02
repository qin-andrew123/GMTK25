using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using UnityEngine.VFX;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField]
    private VisualEffect mGlitchVFX;
    [SerializeField]
    private float mAbilityCooldown;
    private bool bHasTriggeredAbility = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float abilityInput = Input.GetAxisRaw("Ability1");
        if ((abilityInput >= 1.0f) && !bHasTriggeredAbility)
        {
            Debug.Log("Triggered Ability");

            bHasTriggeredAbility = true;
            Glitch();
            StartCoroutine(AbilityCooldown());
        }
    }

    private void Glitch()
    {
        GlitchableObject Target = GlobalVariables.Instance.GlitchManager.GetBestTarget();
        if (!Target)
        {
            Debug.LogWarning("PlayerAbilities.Glitch(): Target Object is null.");
            return;
        }
        Target.GlitchEffect();
    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(mAbilityCooldown);
        Debug.Log("End of Cooldown");
        bHasTriggeredAbility = false;
    }

}
