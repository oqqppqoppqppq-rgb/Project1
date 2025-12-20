using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Vector3[] cameraPositions = new Vector3[4];
    public Vector3[] cameraRotations = new Vector3[4];
    public float moveSpeed = 1f;

    private Coroutine activeCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) MoveToView(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) MoveToView(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) MoveToView(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) MoveToView(3);
    }

    public void MoveToView(int view)
    {
        if (activeCoroutine != null) return;
        activeCoroutine = StartCoroutine(MoveCamera(view));
    }

    IEnumerator MoveCamera(int view)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(cameraRotations[view]);

        float elapsed = 0f;

        while (elapsed < moveSpeed)
        {
            float t = elapsed / moveSpeed;
            mainCamera.transform.position = Vector3.Lerp(startPos, cameraPositions[view], t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = cameraPositions[view];
        mainCamera.transform.rotation = targetRot;
        activeCoroutine = null;
    }
}