using UnityEngine;

/// <summary>
/// move controll -> use CharacterController(Component)
/// rotation controll
/// turn around method
/// </summary>
public class CharacterControll : MonoBehaviour
{
    private static CharacterControll Instance;
    private CharacterController controller;

    //move
    Vector2 moveInput;
    Vector3 move;
    public float moveSpeed = 2.0f;

    //rotation
    Vector2 rotationInput;
    private float rotation;
    public float rotationSpeed = 60.0f;

    private string animationTrigger = "IsWalking";

    public static CharacterControll characterControll
    {
        get { return Instance; }
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Awake()
    {
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

    void Update()
    {
        CharacterMove();
    }
    private void CharacterMove()
    {
        moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);//-1.0~1.0
        rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);//-1.0~1.0

        //Move (VR)
        if ((moveInput.x != 0) || (moveInput.y != 0))
        {
            if (CameraManager.cameraManager.screenCaptureCheck == false)
                move = new Vector3(moveInput.x, 0f, moveInput.y);//Normal Mode
            else
                move = new Vector3(-moveInput.x, 0f, -moveInput.y);//ScreenCapture Mode

            move = CharacterManager.characterManager.characterTransform.TransformDirection(move) * Time.deltaTime * moveSpeed;//convert local to world 
            controller.Move(move);

            CharacterManager.characterManager.chracterAnimator.SetBool(animationTrigger, true);
        }
        else
        {
            CharacterManager.characterManager.chracterAnimator.SetBool(animationTrigger, false);
        }

        //Move (PC)
        if ((Input.GetAxis("Horizontal") != 0) || (Input.GetAxis("Vertical") != 0))
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            move = CharacterManager.characterManager.characterTransform.TransformDirection(move) * Time.deltaTime * moveSpeed * 2;//convert local to world 
            controller.Move(move);
        }

        //Rotation (VR)
        rotation = rotationInput.x * rotationSpeed * Time.deltaTime;
        CharacterManager.characterManager.characterTransform.Rotate(Vector3.up, rotation);

        //Rotation (PC)
        if (Input.GetAxis("Mouse X") != 0)
        {
            rotation = Input.GetAxis("Mouse X") * rotationSpeed * 5 * Time.deltaTime;
            CharacterManager.characterManager.characterTransform.Rotate(Vector3.up, rotation);
        }

    }
    public void CharacterTurnAround()//use screencapture mode
    {
        CharacterManager.characterManager.characterTransform.Rotate(Vector3.up, 180f);
    }

}
