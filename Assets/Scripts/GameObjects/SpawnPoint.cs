using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private string PointName = "";

    private void Awake()
    {
        GlobalVariables.Instance.LevelManager.AddSpawnPoint(this, PointName);
    }
    private void OnDestroy()
    {
        GlobalVariables.Instance.LevelManager.RemoveSpawnPoint(PointName);
    }
}
