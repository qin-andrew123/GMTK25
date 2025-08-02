using System.Collections.Generic;
using UnityEngine;

public class GrabableObjectManager : MonoBehaviour
{
    public static GrabableObjectManager Instance;
    [SerializeField]
    private float mGrabDetectionRadius;
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

        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(float grabbableDistance)
    {
        if (mManagedObjects == null)
        {
            mManagedObjects = new List<GrabableObject>();
        }

        mGrabDetectionRadius = grabbableDistance;
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
            if (testDistance < bestDistance && testDistance <= mGrabDetectionRadius)
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
