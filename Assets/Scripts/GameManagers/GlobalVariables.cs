using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    [SerializeField] private float mGlitchDetectionRadius = 5.0f;
    [SerializeField] private float mGrabDetectionRadius = 3.0f;
    [SerializeField] private Transform mStartingPosition;
    [SerializeField] private GameObject mDarknessVolume;
    [SerializeField] private List<GameObject> mPlatformsToUpdate = new List<GameObject>();
    [SerializeField] private List<GameObject> mSoundBarriersToUpdate = new List<GameObject>();
    [SerializeField] private PlayerMovementData corruptedPlayerMoveData;
    [SerializeField] private PlayerMovementData normalPlayerMoveData;
    private GameObject mPlayerRef = null;
    public static GlobalVariables Instance;
    private LevelManager mLevelManager;
    private GlitchManager mGlitchManager;
    private GrabableObjectManager mGrabableObjectManager;
    private CodeObjectManager mCodeObjectManager;
    public GameObject PlayerRef { get { return mPlayerRef; } }
    public LevelManager LevelManager { get { return mLevelManager; } }
    public GlitchManager GlitchManager { get { return mGlitchManager; } }
    public GrabableObjectManager GrabableObjectManager { get { return mGrabableObjectManager; } }
    public CodeObjectManager CodeObjectManager { get { return mCodeObjectManager; } }
    public GameObject DarknessVolume { get { return mDarknessVolume; } }
    public Transform StartingPosition { get { return mStartingPosition; } }
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

        if(!mCodeObjectManager)
        {
            mCodeObjectManager = gameObject.AddComponent<CodeObjectManager>();
        }
        mCodeObjectManager.Initialize(mGrabDetectionRadius);
    }
    public void HotReloadLevel()
    {
        mLevelManager.HandleReloadLevel();
    }
    public void UpdateGameObjects(bool bEnable)
    {
        if(mPlatformsToUpdate.Count == 0)
        {
            Debug.Log("Gameobjects count is empty but we should be setting inactive/active");
            return;
        }
        foreach(GameObject go in mPlatformsToUpdate)
        {
            go.SetActive(bEnable);
        }
    }

    public void UpdateSound(bool bEnable)
    {
        if(mSoundBarriersToUpdate.Count == 0)
        {
            Debug.Log("sound barriers count is empty but we should be setting inactive/active");
            return;
        }
        foreach (GameObject go in mSoundBarriersToUpdate)
        {
            go.SetActive(bEnable);
        }
    }

    public void UpdatePhysics(bool bEnabled)
    {
        PlayerMovement3D playerMove = mPlayerRef.GetComponent<PlayerMovement3D>();
        if(!playerMove)
        {
            return;
        }

        if(bEnabled)
        {
            normalPlayerMoveData = playerMove.mPlayerData;
            playerMove.mPlayerData = corruptedPlayerMoveData;
        }
        else
        {
            playerMove.mPlayerData = normalPlayerMoveData;
        }
    }
}
