using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagPole : MonoBehaviour
{
    [SerializeField]
    private Transform mRespawnPosition;
    [SerializeField]
    private int mLevelNumber = 0;
    [SerializeField]
    private string mLevelToLoad = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleCollision(other.gameObject);
        }
    }

    private void HandleCollision(GameObject player)
    {
        switch (mLevelNumber)
        {
            default: 
                Debug.LogWarning("No Valid Level Set on Flagpole");
                break;

            case 1:
                GrabableObject grabableObject = player.GetComponentInChildren<GrabableObject>();
                if (grabableObject && grabableObject.CompareTag("Star"))
                {
                    Debug.Log("Finish Level 1!");
                    Debug.Log("Trigger End Glitch Cutscene");
                    SceneManager.LoadScene(mLevelToLoad);
                }
                else
                {
                    // Else, interacting with this specific flagpole results in "failure" (i.e. just glitch respawning you)
                    Debug.Log("Flagpole Failure");
                    Respawn(player);
                }
                break;

            case 2:
                Debug.Log("Finish Level 2!");
                Debug.Log("Load Level 3");
                SceneManager.LoadScene(mLevelToLoad);
                break;

            case 3:
                Debug.Log("Finish Level 3!");
                Debug.Log("Load End of Game");
                SceneManager.LoadScene(mLevelToLoad);
                break;
            case 4:
                Debug.Log("End of Game!");
                SceneManager.LoadScene(mLevelToLoad);
                break;
        }
    }

    private void Respawn(GameObject player)
    {
        player.transform.position = mRespawnPosition.position;
        // TODO: play respawn glitch VFX
    }
}
