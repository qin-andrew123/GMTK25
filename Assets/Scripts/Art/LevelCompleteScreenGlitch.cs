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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mVolume.gameObject.SetActive(false);
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
        yield return new WaitForSeconds(2f);
        mVolume.gameObject.SetActive(true);

        yield return new WaitForSeconds(mScreenTime);
        SceneManager.LoadScene("SecondLevelBlockout");

    }
}
