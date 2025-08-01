using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;

public class PlayerAbilities : MonoBehaviour
{
    private CharacterController mCharacterController;

    [SerializeField]
    private float mMaxGlitchRadius;
    private float mAbilityCooldown;
    private bool bHasTriggeredAbility = false;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float abilityInput = Input.GetAxisRaw("Ability1");
        if((abilityInput >= 1.0f) && !bHasTriggeredAbility)
        {
            Glitch();
            bHasTriggeredAbility = true;
        }
    }

    private void Glitch()
    {
        // Check if in the Radius of a glitchable object via spherecast?
        RaycastHit raycastHit;

        Vector3 position = mCharacterController.center;
        bool hit = Physics.SphereCast(position, mCharacterController.height / 2.0f, transform.forward, out raycastHit, mMaxGlitchRadius);
        if(raycastHit.collider.CompareTag("GlitchableObject"))
        {
            // Trigger success glitch

            // Success glitch vfx

            // Begin cooldown
            StartCoroutine(AbilityCooldown());
        }
        else
        {
            // failure glitch vfx
        }

    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(mAbilityCooldown);
        bHasTriggeredAbility = false;
    }

}
