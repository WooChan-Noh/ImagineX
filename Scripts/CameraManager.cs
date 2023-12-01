using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// singleton pattern
/// camera position -> use Ray
/// manage POV
/// </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager Instance;
    private static float cameraDistanceInstance;
    private static float cameraHeightInstance;
    private static bool thirdPOVInstance;
    private static bool screenCaptureCheckInstance;

    private float maxCameraDistance = 0.21f;
    private float minCameraDistance = -3.0f;
    private float maxCameraHeight = 2.5f;
    private float minCameraHeight = 1.5f;
    private float characterHeight = 1.8f;
    private float cameraDistanceInThirdPOV = 2.0f;

    private Ray ray;
    private RaycastHit hit;

    public static CameraManager cameraManager
    {
        get { return Instance; }
    }
    public bool thirdPOV
    {
        get { return thirdPOVInstance; }
        set
        {
            if ((value == false) || (value == true))
            {
                thirdPOVInstance = value;
            }
            else
                Debug.Log("Wrong value in CameraManager_thirdPOV");
        }
    }
    public bool screenCaptureCheck
    {
        get { return screenCaptureCheckInstance; }
        set
        {
            if ((value == false) || (value == true))
            {
                screenCaptureCheckInstance = value;
            }
            else
                Debug.Log("Wrong value in CameraManager_screenCaptureCheck");
        }
    }
    public float cameraDistance
    {
        get { return cameraDistanceInstance; }
        set
        {
            if (minCameraDistance < value && value < maxCameraDistance)
            {
                cameraDistanceInstance = value;
            }
            else
                Debug.Log("Wrong value in CameraManager_cameraDistance");
        }
    }
    public float cameraHeight
    {
        get { return cameraHeightInstance; }
        set
        {
            if (minCameraHeight < value && value < maxCameraHeight)
            {
                cameraHeightInstance = value;
            }
            else
                Debug.Log("Wrong value in CameraManager_cameraHeight");
        }
    }

    private void Awake()
    {
        thirdPOVInstance = false;
        screenCaptureCheckInstance = false;
        cameraDistanceInstance = 0.2f;
        cameraHeightInstance = 1.8f;

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        ray = new Ray();

    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(ray.origin, hit.point);
    //}

    public void SetCameraPostion()
    {
        if (thirdPOV == false)//firstPOV
        {
            cameraDistance = 0.0f;
            cameraHeight = characterHeight;
        }
        else if (thirdPOV == true)//thirdPOV
        {
            ray.origin = new Vector3(CharacterManager.characterManager.transform.position.x,
                                     CharacterManager.characterManager.transform.position.y + characterHeight,
                                     CharacterManager.characterManager.transform.position.z);

            if (screenCaptureCheckInstance == false)//normal mode
                ray.direction = CharacterManager.characterManager.transform.rotation * Vector3.back;
            else//screencapture mode
                ray.direction = CharacterManager.characterManager.transform.rotation * Vector3.forward;

            Physics.Raycast(ray, out hit);

            if (hit.distance < cameraDistanceInThirdPOV)//Ray hit some object
            {
                cameraDistance = -hit.distance;
                cameraHeight = characterHeight + 0.1f;
            }
            else
            {
                cameraDistance = -cameraDistanceInThirdPOV;
                cameraHeight = characterHeight + 0.1f;
            }
        }
    }

    public Transform MoveCamera(Transform transform)
    {
        if (screenCaptureCheckInstance == false)//normal mode -> backward
            transform.position = CharacterManager.characterManager.characterTransform.position +
                                (CharacterManager.characterManager.characterTransform.forward * cameraDistance) +
                                (Vector3.up * cameraHeight);
        else//screencapture mode -> forward
            transform.position = CharacterManager.characterManager.characterTransform.position +
                                (CharacterManager.characterManager.characterTransform.forward * -cameraDistance) +
                                (Vector3.up * cameraHeight);

        //camera rotation
        if (screenCaptureCheckInstance == false)//normal mode -> front
            transform.rotation = CharacterManager.characterManager.characterTransform.rotation;
        else//screencapture mode -> back
        {
            transform.rotation = CharacterManager.characterManager.characterTransform.rotation;
            transform.Rotate(Vector3.up, 180f);
        }

        return transform;
    }
    //


    //
    public void ChangePOV()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick) || OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
        {
            if (screenCaptureCheckInstance == false)//normal mode
                thirdPOV = !thirdPOV;
            else//screencapture mode -> firstPOV
            {
                screenCaptureCheckInstance = !screenCaptureCheckInstance;
                CharacterControll.characterControll.CharacterTurnAround();
                thirdPOV = !thirdPOV;
            }
        }
    }
}
