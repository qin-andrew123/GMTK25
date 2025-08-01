using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class LevelCompleteScreenGlitch : MonoBehaviour
{
    [SerializeField]
    private float mScreenTime;
    [SerializeField]
    private Volume mVolume;
    [SerializeField]
    private string mSceneToLoad = "LevelBlockout_2";
    [SerializeField]
    private AudioEvent mLevelCompleteStinger;
    [SerializeField]
    private AudioEvent mGlitchSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mVolume.gameObject.SetActive(false);
        mLevelCompleteStinger.Play2DSound();
        StartCoroutine(GlitchAndShutdown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GlitchAndShutdown()
    {
        //float timer = mScreenTime;
        //while(timer > 0.0f)
        //{
        //    mVolume.parameters
        //    yield return null;
        //}
        yield return new WaitForSeconds(1.75f);
        mGlitchSFX.Play2DSound();
        yield return new WaitForSeconds(0.25f);
        mVolume.gameObject.SetActive(true);

        yield return new WaitForSeconds(mScreenTime);
        SceneManager.LoadScene(mSceneToLoad);
    }
}
