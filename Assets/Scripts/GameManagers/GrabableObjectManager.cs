using System.Collections.Generic;
using UnityEngine;

public class GrabableObjectManager : MonoBehaviour
{
    public static GrabableObjectManager Instance;
    [SerializeField]
    private float DetectionRadius;
    public List<GrabableObject> mManagedObjects = new List<GrabableObject>();
    private GrabableObject bestCandidate = null;
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

    public void AddGrabableObject(GrabableObject grabableObject)
    {
        if (mManagedObjects.Contains(grabableObject))
        {
            return;
        }
        mManagedObjects.Add(grabableObject);
    }

    public void RemoveGrabableObject(GrabableObject grabableObject)
    {
        if (!mManagedObjects.Contains(grabableObject))
        {
            return;
        }
        mManagedObjects.Remove(grabableObject);
    }
    public GrabableObject GetBestTarget()
    {
        return bestCandidate;
    }
    private void Update()
    {
        GetClosestGrabableObjectToPlayer(mPlayerRef.transform.position);
    }
    private void GetClosestGrabableObjectToPlayer(Vector3 playerPosition)
    {
        bestCandidate = null;
        float bestDistance = float.MaxValue;
        foreach (GrabableObject obj in mManagedObjects)
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
