using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlitchableObject : MonoBehaviour
{

    [SerializeField] private string mSceneToLoad; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerGlitch()
    {
        // triggering a glitch sends you to a certain scene
        //SceneManager.LoadScene(mSceneToLoad);
    }
}
