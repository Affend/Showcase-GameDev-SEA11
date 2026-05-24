using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void Start()
    {
        // If the player exists, teleport them instantly to this object's exact position
        if (Movement.Instance != null)
        {
            Movement.Instance.transform.position = transform.position;
        }
    }
}