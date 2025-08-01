using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private Dictionary<string, SpawnPoint> mSpawnPoints;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(Transform startingPosition)
    {
        if (mSpawnPoints == null)
        {
            mSpawnPoints = new Dictionary<string, SpawnPoint>();
        }

        MovePlayerToPosition(startingPosition);
    }
    public void AddSpawnPoint(SpawnPoint point, string pointName)
    {
        if(mSpawnPoints.ContainsKey(pointName))
        {
            return;
        }

        mSpawnPoints.Add(pointName, point);
    }

    public void RemoveSpawnPoint(string pointName)
    {
        if (!mSpawnPoints.ContainsKey(pointName))
        {
            return;
        }

        SpawnPoint targetSpawnPoint = mSpawnPoints[pointName];
        mSpawnPoints.Remove(pointName);
        Destroy(targetSpawnPoint);
    }
    public SpawnPoint TryGetSpawnPoint(string pointName)
    {
        if (!mSpawnPoints.ContainsKey(pointName))
        {
            return null;
        }

        return mSpawnPoints[pointName];
    }

    public void MovePlayerToSpawnPoint(string pointName)
    {
        SpawnPoint spawnPoint = TryGetSpawnPoint(pointName);
        if (!spawnPoint)
        {
            return;
        }

        GlobalVariables.Instance.PlayerRef.transform.position = spawnPoint.transform.position;
    }
    public void MovePlayerToPosition(Transform transform)
    {
        GlobalVariables.Instance.PlayerRef.transform.position = transform.position;
    }
    public void HandleMovePlayerToNewLevel(string LevelName)
    {
        SceneManager.LoadScene(LevelName);

    }
    public void HandleReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
