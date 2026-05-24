using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    private int nextSceneIndex;

    void Start()
    {
        // Calculate the next scene index safely on start
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }

    // This runs automatically when another BoxCollider2D enters this object's trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Optional safety: Check if the object entering the door is actually the Player
        if (other.CompareTag("Player"))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        // Safety check: Make sure the next scene actually exists in Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available in Build Settings!");
        }
    }
}