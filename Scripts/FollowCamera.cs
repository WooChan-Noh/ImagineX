using UnityEngine;

/// <summary>
/// controll POV status
/// camera move controll
/// </summary>
public class FollowCamera : MonoBehaviour
{

    private Transform cameraTransform;

    void Update()
    {
        CameraManager.cameraManager.ChangePOV();
    }
    void LateUpdate()
    {
        CameraMovement();
    }
    private void CameraMovement()
    {
        CameraManager.cameraManager.SetCameraPostion();
        cameraTransform = CameraManager.cameraManager.MoveCamera(transform);

        transform.position = cameraTransform.position + new Vector3(0, -0.12f, 0);
        transform.rotation = cameraTransform.rotation;
    }
}



