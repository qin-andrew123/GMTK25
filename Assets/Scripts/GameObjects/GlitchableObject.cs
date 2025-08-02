using System.Collections;
using TMPro;
using UnityEngine;

public class GlitchableObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro mInteractText;
    [SerializeField, Tooltip("The name of the spawnpoint that you want to transport the player to when you glitch")]
    private string mLevelToGlitchTo = "";
    void Awake()
    {
        GlobalVariables.Instance.GlitchManager.AddGlitchableObject(this);
    }
    private void OnDestroy()
    {
        GlobalVariables.Instance.GlitchManager.RemoveGlitchableObject(this);
    }
    public void ChangeInteractTextStatus(bool bShouldbeEnabled)
    {
        mInteractText.enabled = bShouldbeEnabled;
    }
    public void GlitchEffect()
    {
        Debug.Log("This is a glitch effect");
        if(mLevelToGlitchTo.Length == 0)
        {
            Debug.LogWarning(gameObject.name + " has an empty string for mLevelToGlitchTo");
            return;
        }
        GlobalVariables.Instance.LevelManager.HandleMovePlayerToNewLevel(mLevelToGlitchTo);
    }

    private IEnumerator AddSelfToManager()
    {
        yield return new WaitUntil(() => GlobalVariables.Instance.GlitchManager != null);
        GlobalVariables.Instance.GlitchManager.AddGlitchableObject(this);
    }
}
