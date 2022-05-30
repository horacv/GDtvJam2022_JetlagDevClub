using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] List<GameObject> respawnPoints;

    [SerializeField] int currentCheckpoint;

    public Transform GetRespawnPoint()
    {
        return respawnPoints[currentCheckpoint].transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Checkpoint"))
        {
            int pointIndex = respawnPoints.IndexOf(other.gameObject);
            if (pointIndex > currentCheckpoint)
                currentCheckpoint = pointIndex;
        }
    }
}
