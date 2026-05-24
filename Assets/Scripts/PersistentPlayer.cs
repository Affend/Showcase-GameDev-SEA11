using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    // Static reference to hold our single player instance
    public static PersistentPlayer Instance { get; private set; }

    void Awake()
    {
        // If an instance already exists and it's not me, destroy myself.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, make this the official instance and protect it from destruction
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}