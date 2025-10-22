using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotationDuration = 0.5f;

    [Header("Wall References")]
    public Transform awEastTEST;
    public Transform awWestTEST;
    public Transform awSouthTEST;
    public Transform awNorthTEST;
    public Transform floor;

    [Header("Wall Positions")]
    public Vector3 wallActivePosition = new Vector3(0, 1.5566f, 0);
    public Vector3 wallHiddenPosition = new Vector3(0, 10f, 0);
    public Vector3 wallScaleActive = new Vector3(0.1116f, 3.10971f, 14f);
    public Vector3 wallNorthSouthScale = new Vector3(0.1116f, 3.10971f, 8.205259f);

    public enum RoomView
    {
        NorthEast = 0, // Вид 1 - Север и Восток
        SouthEast = 1, // Вид 2 - Юг и Восток
        SouthWest = 2, // Вид 3 - Юг и Запад
        NorthWest = 3  // Вид 4 - Север и Запад
    }

    private RoomView currentView = RoomView.NorthEast;
    private bool isRotating = false;

    // Позиции камеры для каждого вида
    private readonly Vector3[] cameraPositions =
    {
        new Vector3(5.69f, 6.14f, -7.56f),    // NorthEast - Вид 1
        new Vector3(5.69f, 6.14f, 7.56f),     // SouthEast - Вид 2
        new Vector3(-5.69f, 6.14f, 7.56f),    // SouthWest - Вид 3
        new Vector3(-5.69f, 6.14f, -7.56f)    // NorthWest - Вид 4
    };

    // Повороты камеры для каждого вида
    private readonly Quaternion[] cameraRotations =
    {
        Quaternion.Euler(24.408f, -53.281f, 0.009f),  // NorthEast - Вид 1
        Quaternion.Euler(24.408f, -126.719f, 0.009f), // SouthEast - Вид 2
        Quaternion.Euler(24.408f, 126.719f, 0.009f),  // SouthWest - Вид 3
        Quaternion.Euler(24.408f, 53.281f, 0.009f)    // NorthWest - Вид 4
    };

    void Start()
    {
        
        InitializeRoom();
    }

    void Update()
    {
        
        HandleInput();
    }

    void InitializeRoom()
    {
        // Начальное положение камеры
        UpdateWallsForCurrentView();
    }

    void HandleInput()
    {
        if (isRotating) return;

        // Управление цифрами 1, 2, 3, 4 для переключения видов

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToView(RoomView.NorthEast);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToView(RoomView.SouthEast);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToView(RoomView.SouthWest);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchToView(RoomView.NorthWest);
        }

        // Дополнительное управление стрелками для последовательного вращения

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateRoomLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateRoomRight();
        }
    }

    public void SwitchToView(RoomView targetView)
    {
        if (isRotating || currentView == targetView) return;

        StartCoroutine(SwitchToViewCoroutine(targetView));
    }

    IEnumerator SwitchToViewCoroutine(RoomView targetView)
    {
        isRotating = true;

        
        HideAllWallsExcept(targetView);

        // Начальные и конечные позиции камеры

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 targetPos = cameraPositions[(int)targetView];
        Quaternion targetRot = cameraRotations[(int)targetView];

        
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;

            
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        
        transform.position = targetPos;
        transform.rotation = targetRot;
        currentView = targetView;

        isRotating = false;
    }

    void HideAllWallsExcept(RoomView targetView)
    {
        
        awEastTEST.position = wallHiddenPosition;
        awWestTEST.position = wallHiddenPosition;
        awSouthTEST.position = wallHiddenPosition;
        awNorthTEST.position = wallHiddenPosition;

        
        switch (targetView)
        {
            case RoomView.NorthEast:
                StartCoroutine(AnimateWall(awNorthTEST, wallHiddenPosition, GetNorthWallPosition(), rotationDuration));
                StartCoroutine(AnimateWall(awEastTEST, wallHiddenPosition, GetEastWallPosition(), rotationDuration));
                break;
            case RoomView.SouthEast:
                StartCoroutine(AnimateWall(awSouthTEST, wallHiddenPosition, GetSouthWallPosition(), rotationDuration));
                StartCoroutine(AnimateWall(awEastTEST, wallHiddenPosition, GetEastWallPosition(), rotationDuration));
                break;
            case RoomView.SouthWest:
                StartCoroutine(AnimateWall(awSouthTEST, wallHiddenPosition, GetSouthWallPosition(), rotationDuration));
                StartCoroutine(AnimateWall(awWestTEST, wallHiddenPosition, GetWestWallPosition(), rotationDuration));
                break;
            case RoomView.NorthWest:
                StartCoroutine(AnimateWall(awNorthTEST, wallHiddenPosition, GetNorthWallPosition(), rotationDuration));
                StartCoroutine(AnimateWall(awWestTEST, wallHiddenPosition, GetWestWallPosition(), rotationDuration));
                break;
        }
    }

    void UpdateWallsForCurrentView()
    {
        
        awEastTEST.position = wallHiddenPosition;
        awWestTEST.position = wallHiddenPosition;
        awSouthTEST.position = wallHiddenPosition;
        awNorthTEST.position = wallHiddenPosition;

        
        switch (currentView)
        {
            case RoomView.NorthEast:
                awNorthTEST.position = GetNorthWallPosition();
                awEastTEST.position = GetEastWallPosition();
                break;
            case RoomView.SouthEast:
                awSouthTEST.position = GetSouthWallPosition();
                awEastTEST.position = GetEastWallPosition();
                break;
            case RoomView.SouthWest:
                awSouthTEST.position = GetSouthWallPosition();
                awWestTEST.position = GetWestWallPosition();
                break;
            case RoomView.NorthWest:
                awNorthTEST.position = GetNorthWallPosition();
                awWestTEST.position = GetWestWallPosition();
                break;
        }
    }

    public void RotateRoomLeft()
    {
        if (isRotating) return;

        RoomView targetView = (RoomView)(((int)currentView + 3) % 4); // -1 по модулю 4
        SwitchToView(targetView);
    }

    public void RotateRoomRight()
    {
        if (isRotating) return;

        RoomView targetView = (RoomView)(((int)currentView + 1) % 4); // +1 по модулю 4
        SwitchToView(targetView);
    }

    IEnumerator AnimateWall(Transform wall, Vector3 from, Vector3 to, float duration)
        {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Плавное движение с ease-in-out
            t = Mathf.SmoothStep(0f, 1f, t);
            
            wall.position = Vector3.Lerp(from, to, t);
            
            yield return null;
        }
        
        wall.position = to;
    }


    Vector3 GetNorthWallPosition()
    {
        return new Vector3(-0.02f, 1.5566f, 7.08f);
    }

    Vector3 GetSouthWallPosition()
    {
        return new Vector3(-0.01654f, 1.5566f, -7.078f);
    }

    Vector3 GetEastWallPosition()
    {
        return new Vector3(-4f, 1.5566f, 0.014f);
    }

    Vector3 GetWestWallPosition()
    {
        return new Vector3(4f, 1.5566f, 0.014f);
    }

    // Методы для UI кнопок
    public void OnView1Button()
    {
        SwitchToView(RoomView.NorthEast);
    }

    public void OnView2Button()
    {
        SwitchToView(RoomView.SouthEast);
    }

    public void OnView3Button()
    {
        SwitchToView(RoomView.SouthWest);
    }

    public void OnView4Button()
    {
        SwitchToView(RoomView.NorthWest);
    }

    public void OnLeftRotationButton()
    {
        RotateRoomLeft();
    }

    public void OnRightRotationButton()
    {
        RotateRoomRight();
    }

    public void SetView(int viewIndex)
    {
        if (viewIndex >= 0 && viewIndex < 4 && !isRotating)
        {
            RoomView targetView = (RoomView)viewIndex;
            SwitchToView(targetView);
        }
    }
}

// НА ДАННЫЙ МОМЕНТ ЯВЛЯЕТСЯ ИСКЛЮЧИТЕЛЬНО ТЕСТ-КОДОМ, ПРОВЕРКОЙ РЕАЛИЗАЦИИ ИДЕИ