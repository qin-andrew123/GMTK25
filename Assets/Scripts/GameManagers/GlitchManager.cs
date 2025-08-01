using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public static GlitchManager Instance;
    [SerializeField]
    private float DetectionRadius;
    public List<GlitchableObject> mManagedObjects = new List<GlitchableObject>();
    private GlitchableObject bestCandidate = null;
    private GameObject mPlayerRef = null;
    void Awake()
    {
        if (Instance != null)
        {
            Instance = null;
        }

        Instance = this;

        if (!mPlayerRef)
        {
            mPlayerRef = GameObject.FindGameObjectWithTag("Player");
        }
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
    }
    public GlitchableObject GetBestTarget()
    {
        return bestCandidate;
    }
    private void Update()
    {
        GetClosestGlitchableObjectToPlayer(mPlayerRef.transform.position);
    }
    private void GetClosestGlitchableObjectToPlayer(Vector3 playerPosition)
    {
        bestCandidate = null;
        float bestDistance = float.MaxValue;
        foreach (GlitchableObject obj in mManagedObjects)
        {
            float testDistance = Vector3.Distance(playerPosition, obj.transform.position);
            if (testDistance < bestDistance && testDistance <= DetectionRadius)
            {
                bestCandidate = obj;
                bestDistance = testDistance;
                bestCandidate.ChangeInteractTextStatus(true);
            }
            else
            {
                obj.ChangeInteractTextStatus(false);
            }
        }
    }
}
