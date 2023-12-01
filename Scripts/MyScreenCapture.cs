using UnityEngine;

/// <summary>
/// screencapture
/// and character turn(only thirdPOV)
/// </summary>
public class MyScreenCapture : MonoBehaviour
{
    private string fileName;

    void Update()
    {
        StartScreenCapture();
        ScreenCaptureMode();
    }
    void StartScreenCapture()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            fileName = "MyCapture_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(fileName, ScreenCapture.StereoScreenCaptureMode.BothEyes); //set path?
        }
    }

    void ScreenCaptureMode()
    {
        if (CameraManager.cameraManager.thirdPOV == true)//active only thirdPOV
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A) || OVRInput.GetDown(OVRInput.RawButton.X))
            {
                CameraManager.cameraManager.screenCaptureCheck = !CameraManager.cameraManager.screenCaptureCheck;
                CharacterControll.characterControll.CharacterTurnAround();
            }
        }

    }

}
