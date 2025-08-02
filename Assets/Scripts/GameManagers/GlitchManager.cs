using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    private float mGlitchableDistance = 5.0f;
    public List<GlitchableObject> mManagedObjects;
    private GlitchableObject bestCandidate = null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(float glitchableDistance)
    {
        if (mManagedObjects == null)
        {
            mManagedObjects = new List<GlitchableObject>();
        }

        mGlitchableDistance = glitchableDistance;
    }

    public void AddGlitchableObject(GlitchableObject glitchableObject)
    {
        if (mManagedObjects.Contains(glitchableObject))
        {
            return;
        }
        mManagedObjects.Add(glitchableObject);
    }

    public void RemoveGlitchableObject(GlitchableObject glitchableObject)
    {
        if (!mManagedObjects.Contains(glitchableObject))
        {
            return;
        }
        mManagedObjects.Remove(glitchableObject);
        Destroy(glitchableObject);
    }
    public GlitchableObject GetBestTarget()
    {
        return bestCandidate;
    }
    private void Update()
    {
        GetClosestGlitchableObjectToPlayer(GlobalVariables.Instance.PlayerRef.transform.position);
    }
    private void GetClosestGlitchableObjectToPlayer(Vector3 playerPosition)
    {
        bestCandidate = null;
        float bestDistance = float.MaxValue;
        foreach (GlitchableObject obj in mManagedObjects)
        {
            float testDistance = Vector3.Distance(playerPosition, obj.transform.position);
            if (testDistance < bestDistance && testDistance <= mGlitchableDistance)
            {
                bestCandidate = obj;
                bestDistance = testDistance;
            }
            obj.ChangeInteractTextStatus(false);
        }
        if (bestCandidate)
        {
            bestCandidate.ChangeInteractTextStatus(true);
        }
    }
}
