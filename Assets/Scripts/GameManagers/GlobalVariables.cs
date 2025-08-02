using NUnit.Framework;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{ 
    [SerializeField] private float mGlitchDetectionRadius = 5.0f;
    [SerializeField] private Vector3 mStartingPosition = new Vector3(0, 1, 0);
    private GameObject mPlayerRef = null;
    public static GlobalVariables Instance;
    private LevelManager mLevelManager;
    private GlitchManager mGlitchManager;

    public GameObject PlayerRef { get { return mPlayerRef; } }
    public LevelManager LevelManager { get { return mLevelManager; } }
    public GlitchManager GlitchManager { get { return mGlitchManager; } }
    private void Awake()
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

        if (!mLevelManager)
        {
            mLevelManager = gameObject.AddComponent<LevelManager>();
        }
        mLevelManager.Initialize(mStartingPosition);

        if (!mGlitchManager)
        {
            mGlitchManager = gameObject.AddComponent<GlitchManager>();
        }
        mGlitchManager.Initialize(mGlitchDetectionRadius);
    }

}
