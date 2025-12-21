using UnityEngine;

public class CameraZoomToObject : MonoBehaviour
{
    [Header("Камера для перемещения")]
    public Camera targetCamera;

    [Header("Позиция при клике")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isZoomed = false;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        originalPosition = targetCamera.transform.position;
        originalRotation = targetCamera.transform.rotation;
    }

    void OnMouseDown()
    {
        if (!isZoomed)
        {
            targetCamera.transform.position = cameraPosition;
            targetCamera.transform.rotation = Quaternion.Euler(cameraRotation);
            isZoomed = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isZoomed)
        {
            targetCamera.transform.position = originalPosition;
            targetCamera.transform.rotation = originalRotation;
            isZoomed = false;
        }
    }
}