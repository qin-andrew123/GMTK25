using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using System;

public class PlayerAbilities : MonoBehaviour
{
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
            Glitch();
            bHasTriggeredAbility = true;
        }
    }

    private void Glitch()
    {
        GlitchableObject Target = GlitchManager.Instance.GetBestTarget();
        if (!Target)
        {
            Debug.LogWarning("PlayerAbilities.Glitch(): Target Object is null.");
            return;
        }

        Target.GlitchEffect();
        StartCoroutine(AbilityCooldown());
    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(mAbilityCooldown);
        bHasTriggeredAbility = false;
    }

}
