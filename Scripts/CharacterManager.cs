using UnityEngine;

/// <summary>
/// character transform information
/// manage character animation
/// manage character rendering
/// </summary>
public class CharacterManager : MonoBehaviour
{
    private static CharacterManager Instance;
    private static Transform characterInstance;
    private static Animator animatorInstance;//Uncheck Root Motion

    public static CharacterManager characterManager
    {
        get { return Instance; }
    }
    public Transform characterTransform
    {
        get { return characterInstance.transform; }
    }
    public Animator chracterAnimator
    {
        get { return animatorInstance; }
    }

    private void Awake()
    {
        animatorInstance = GetComponent<Animator>();

        characterInstance = this.transform;

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
    private void Update()
    {
        characterInstance = transform;
        CharacterRendering(transform);
    }
    private void CharacterRendering(Transform transform)
    {
        if (CameraManager.cameraManager.thirdPOV == false)
        {
            foreach (Transform child in transform)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

                if (skinnedMeshRenderer != null)
                {
                    skinnedMeshRenderer.enabled = false;
                }
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();

                if (skinnedMeshRenderer != null)
                {
                    skinnedMeshRenderer.enabled = true;
                }
            }
        }
    }

}
