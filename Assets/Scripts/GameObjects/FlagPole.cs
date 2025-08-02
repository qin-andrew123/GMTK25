using UnityEngine;

public class FlagPole : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPosition;


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
            GrabableObject grabableObject = other.gameObject.GetComponentInChildren<GrabableObject>();
            if (grabableObject && grabableObject.CompareTag("Star")) // Add: "AND is level 1"
            {
                // TODO: Check if this is the first time playing the game.
                // If yes, then Trigger level complete then glitch "cutscene"

                Debug.Log("Win Level 1!");
                Debug.Log("Trigger End Glitch Cutscene");
            }
            else
            {
                // Else, interacting with this specific flagpole results in "failure" (i.e. just glitch respawning you)
                Debug.Log("Flagpole Failure");
                Respawn(other.gameObject);
            }
        }
    }

    private void Respawn(GameObject player)
    {
        player.transform.position = respawnPosition.position;
        // TODO: play respawn glitch VFX
    }
}
