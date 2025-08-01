using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;

public class PlayerAbilities : MonoBehaviour
{
    private CapsuleCollider mCapsuleCollider;

    [SerializeField] private float mMaxGlitchRadius;
    [SerializeField] private float mAbilityCooldown;
    private bool bHasTriggeredAbility = false;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCapsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float abilityInput = Input.GetAxisRaw("Ability1");
        if(!bHasTriggeredAbility && (abilityInput >= 1.0f))
        {
            Debug.Log("Triggered Ability");

            bHasTriggeredAbility = true;
            Glitch();
        }
    }

    private void Glitch()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, (mCapsuleCollider.radius + mMaxGlitchRadius));
        for(int i=0; i<colliders.Length; i++) 
        {
            Collider collider = colliders[i];
            if (collider.CompareTag("GlitchableObject"))
            {
                // Trigger success glitch
                Debug.Log("Glitch!");

                // Success glitch vfx

                // Begin cooldown
                StartCoroutine(AbilityCooldown());
                return;
            }
        }

        // failure glitch vfx
        Debug.Log("No Glitch");
        bHasTriggeredAbility = false;
    }

    private IEnumerator AbilityCooldown()
    {
        yield return new WaitForSeconds(mAbilityCooldown);
        Debug.Log("End of Cooldown");
        bHasTriggeredAbility = false;
    }

}
