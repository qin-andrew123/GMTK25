using NUnit.Framework;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{ 
    [SerializeField] private float mGlitchDetectionRadius = 5.0f;
    [SerializeField] private float mGrabDetectionRadius = 3.0f;
    [SerializeField] private Transform mStartingPosition;
    private GameObject mPlayerRef = null;
    public static GlobalVariables Instance;
    private LevelManager mLevelManager;
    private GlitchManager mGlitchManager;
    private GrabableObjectManager mGrabableObjectManager;

    public GameObject PlayerRef { get { return mPlayerRef; } }
    public LevelManager LevelManager { get { return mLevelManager; } }
    public GlitchManager GlitchManager { get { return mGlitchManager; } }
    public GrabableObjectManager GrabableObjectManager { get { return mGrabableObjectManager; } }
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

        if (!mGrabableObjectManager)
        {
            mGrabableObjectManager = gameObject.AddComponent<GrabableObjectManager>();
        }
        mGrabableObjectManager.Initialize(mGrabDetectionRadius);

        DontDestroyOnLoad(gameObject);
    }
}
