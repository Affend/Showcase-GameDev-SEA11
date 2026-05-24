using UnityEngine;
using Unity.Cinemachine; 
//persistent CameraFollowSetup
public class CameraFollowSetup : MonoBehaviour
{
    void Start()
    {
        // Use our global Instance from the Movement script to find the live player
        if (Movement.Instance != null)
        {
            CinemachineCamera vcam = GetComponent<CinemachineCamera>(); // Or CinemachineVirtualCamera
            if (vcam != null)
            {
                vcam.Follow = Movement.Instance.transform;
                vcam.LookAt = Movement.Instance.transform;
            }
        }
          DontDestroyOnLoad(gameObject);
    }
}