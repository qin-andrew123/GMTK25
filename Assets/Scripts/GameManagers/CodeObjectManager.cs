using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CodeObjectManager : MonoBehaviour
{
    private float mInteractDistance = 5.0f;
    public List<CodeSocket> mManagedObjects;
    private CodeSocket bestCandidate = null;
    
    public void Initialize(float glitchableDistance)
    {
        if (mManagedObjects == null)
        {
            mManagedObjects = new List<CodeSocket>();
        }

        mInteractDistance = glitchableDistance;
    }

    public void AddGlitchableObject(CodeSocket glitchableObject)
    {
        if (mManagedObjects.Contains(glitchableObject))
        {
            return;
        }
        mManagedObjects.Add(glitchableObject);
    }

    public void RemoveGlitchableObject(CodeSocket glitchableObject)
    {
        if (!mManagedObjects.Contains(glitchableObject))
        {
            return;
        }
        mManagedObjects.Remove(glitchableObject);
        Destroy(glitchableObject);
    }
    public CodeSocket GetBestTarget()
    {
        return bestCandidate;
    }

    public void EnableGlitch(CodeSocket socketToEnable)
    {
        foreach(CodeSocket obj in mManagedObjects)
        {
            if(obj == socketToEnable)
            {
                obj.HasBeenGlitched = true;
                ModifyEffects(obj.EffectType, true);
                AudioManager.Instance.CodeGlitchSFX.Play2DSound();
            }
        }
    }

    public void DisableGlitches()
    {
        foreach (CodeSocket obj in mManagedObjects)
        {
            obj.HasBeenGlitched = false;
            ModifyEffects(obj.EffectType, false);
        }
    }
    private void ModifyEffects(GlitchEffectType type, bool bSetEnable)
    {
        switch(type)
        {
            case GlitchEffectType.CHANGE_OBJECTS:
                GlobalVariables.Instance.UpdateGameObjects(!bSetEnable);
                break;
            case GlitchEffectType.CHANGE_SOUND:
                GlobalVariables.Instance.UpdateSound(!bSetEnable);
                break;
            case GlitchEffectType.CHANGE_PHYSICS:
                GlobalVariables.Instance.UpdatePhysics(bSetEnable);
                break;
        }
    }
    private void Update()
    {
        GetClosestGlitchableObjectToPlayer(GlobalVariables.Instance.PlayerRef.transform.position);
    }
    private void GetClosestGlitchableObjectToPlayer(Vector3 playerPosition)
    {
        bestCandidate = null;
        float bestDistance = float.MaxValue;
        foreach (CodeSocket obj in mManagedObjects)
        {
            float testDistance = Vector3.Distance(playerPosition, obj.transform.position);
            if (testDistance < bestDistance && testDistance <= mInteractDistance)
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
