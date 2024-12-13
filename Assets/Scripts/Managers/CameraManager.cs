using UnityEngine;
public enum eCameraPositions { main, center, overhead }
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [NamedArray(typeof(eCameraPositions))] public Transform[] cameraPositions; // For main and center
    [SerializeField] private Transform[] overheadPositions; // Array for overhead positions
    [SerializeField] private eCameraPositions curPosition;
    [SerializeField] private Transform target; // Player target for tracking
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float centerViewFOV = 40f; // FOV for the center camera view
    [SerializeField] private float defaultFOV = 60f;   // Default FOV for other views

    private void Awake()
    {
        Instance = this;
        cameraTransform = Camera.main.transform;
    }

    public void SetCurrentCamera(eCameraPositions cameraPosition, Transform targetTransform = null, int spotIndex = -1)
    {
        curPosition = cameraPosition;

        switch (cameraPosition)
        {
            case eCameraPositions.main:
                ResetCameraParent();
                SetMainCamera();
                AdjustFieldOfView(defaultFOV); // Use the default FOV
                break;

            case eCameraPositions.center:
                ResetCameraParent();
                SetCenterCamera(targetTransform);
                AdjustFieldOfView(centerViewFOV); // Use the center-specific FOV
                break;

            case eCameraPositions.overhead:
                if (spotIndex >= 0 && spotIndex < overheadPositions.Length)
                {
                    SetOverheadCamera(spotIndex);
                    AdjustFieldOfView(defaultFOV); // Use the default FOV
                }
                else if (targetTransform != null)
                {
                    Debug.Log("Overhead camera targetTransform provided but spotIndex preferred.");
                }
                else
                {
                    Debug.Log("Invalid spotIndex for overhead camera or no targetTransform provided.");
                }
                break;

            default:
                Debug.Log($"Unhandled camera position: {cameraPosition}");
                break;
        }
    }

    private void AdjustFieldOfView(float newFOV)
    {
        Camera.main.fieldOfView = newFOV;
    }

    private void ResetCameraParent()
    {
        cameraTransform.SetParent(null);
    }

    private void SetMainCamera()
    {
        if (cameraPositions[(int)eCameraPositions.main] != null)
        {
            Transform mainTransform = cameraPositions[(int)eCameraPositions.main];
            cameraTransform.position = mainTransform.position;
            cameraTransform.rotation = mainTransform.rotation;
        }
        else
        {
            Debug.Log("Main camera position not set in inspector.");
        }
    }

    private void SetCenterCamera(Transform targetTransform)
    {
        if (cameraPositions[(int)eCameraPositions.center] != null)
        {
            target = targetTransform; // Set target to the player piece
            Transform centerTransform = cameraPositions[(int)eCameraPositions.center];
            cameraTransform.position = centerTransform.position; // Initial position
            cameraTransform.rotation = centerTransform.rotation; // Set rotation
        }
        else
        {
            Debug.Log("Center camera position not set in inspector.");
        }
    }

    private void SetOverheadCamera(int spotIndex)
    {
        Transform overheadTransform = overheadPositions[spotIndex];
        if (overheadTransform != null)
        {
            cameraTransform.SetParent(overheadTransform);
            cameraTransform.localPosition = Vector3.zero; // Align with the target's position
            cameraTransform.localRotation = Quaternion.identity; // Match the target's rotation
        }
        else
        {
            Debug.Log($"Overhead position for spotIndex {spotIndex} is not set.");
        }
    }

    private void LateUpdate()
    {
        if (curPosition == eCameraPositions.center && target != null)
        {
            // Rotate the camera to look at the target
            Vector3 direction = target.position - cameraTransform.position; // Direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up); // Calculate rotation
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, 5f * Time.deltaTime); // Smooth rotation
        }
    }
}