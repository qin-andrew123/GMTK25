using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioEvent mGrassFootsteps;
    [SerializeField] private AudioEvent mGenericFoosteps;
    [SerializeField] private LayerMask mLayerMask;
    private PlayerMovement3D mPlayerMovement;
    private string mPrevTag = "Grass";

    private void Awake()
    {
        mPlayerMovement = GetComponent<PlayerMovement3D>();
    }

    public void PlayFootstep()
    {
        RaycastHit info = new();
        if (Physics.Raycast(transform.position, Vector3.down, out info, 3f, mLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (info.collider.CompareTag("Grass"))
            {
                mGrassFootsteps?.Play2DSound();
            }
            else
            {
                mGenericFoosteps?.Play2DSound();
            }
            mPrevTag = info.collider.tag;
        }
        else
        {
            if (mPrevTag == "Grass")
            {
                mGrassFootsteps?.Play2DSound();
            }
            else
            {
                mGenericFoosteps?.Play2DSound();
            }
        }
    }
}
