using UnityEngine;

public class Spike : MonoBehaviour
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
            Respawn(other.gameObject);
        }
    }

    private void Respawn(GameObject player)
    {
        player.transform.position = respawnPosition.position;
        AudioManager.Instance.GlobalHurtSFX.Play2DSound();
        // TODO: play respawn glitch VFX
    }
}
