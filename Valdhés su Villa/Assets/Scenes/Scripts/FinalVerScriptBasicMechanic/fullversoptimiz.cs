using UnityEngine;

public class RoomSystemComplete : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Vector3[] cameraPositions = new Vector3[4];
    public Vector3[] cameraRotations = new Vector3[4];
    public float moveSpeed = 0.5f;

    [Header("Walls Setup")]
    public Transform wallNorth;      // Стена 0
    public Transform wallWest;       // Стена 1
    public Transform wallSouth;      // Стена 2
    public Transform wallEast;       // Стена 3

    [Header("Objects for North Wall (0)")]
    public Transform[] northWallObjects;

    [Header("Objects for West Wall (1)")]
    public Transform[] westWallObjects;

    [Header("Objects for South Wall (2)")]
    public Transform[] southWallObjects;

    [Header("Objects for East Wall (3)")]
    public Transform[] eastWallObjects;

    private bool isMoving = false;
    private int currentView = 0;
    private Vector3 upPosition = new Vector3(0, 100, 0);
    private Vector3[] wallDownPositions = new Vector3[4];

    void Start()
    {
        SaveWallPositions();
        SetupInitialState();
    }

    void SaveWallPositions()
    {
        if (wallNorth != null) wallDownPositions[0] = wallNorth.position;
        if (wallWest != null) wallDownPositions[1] = wallWest.position;
        if (wallSouth != null) wallDownPositions[2] = wallSouth.position;
        if (wallEast != null) wallDownPositions[3] = wallEast.position;
    }

    void SetupInitialState()
    {
        MoveAllWallsUp();
        ShowView(0);
    }

    void MoveAllWallsUp()
    {
        if (wallNorth != null) wallNorth.position = upPosition;
        if (wallWest != null) wallWest.position = upPosition;
        if (wallSouth != null) wallSouth.position = upPosition;
        if (wallEast != null) wallEast.position = upPosition;

        HideAllObjects();
    }

    void Update()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToView(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToView(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToView(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchToView(3);
    }

    void SwitchToView(int newView)
    {
        if (isMoving || newView == currentView) return;
        StartCoroutine(ChangeRoomView(newView));
    }

    System.Collections.IEnumerator ChangeRoomView(int newView)
    {
        isMoving = true;

        // Получаем какие стены показывать/скрывать
        int[] wallsToShow = GetWallsToShow(newView);
        int[] wallsToHide = GetWallsToHide(currentView);

        Vector3 startCamPos = mainCamera.transform.position;
        Quaternion startCamRot = mainCamera.transform.rotation;
        Quaternion targetCamRot = Quaternion.Euler(cameraRotations[newView]);

        float elapsed = 0f;

        while (elapsed < moveSpeed)
        {
            float progress = elapsed / moveSpeed;

            // Двигаем камеру
            mainCamera.transform.position = Vector3.Lerp(startCamPos, cameraPositions[newView], progress);
            mainCamera.transform.rotation = Quaternion.Lerp(startCamRot, targetCamRot, progress);

            // Двигаем стены и объекты
            foreach (int wallIndex in wallsToHide)
            {
                MoveWallDown(wallIndex, progress, false); // Поднимаем
            }

            foreach (int wallIndex in wallsToShow)
            {
                MoveWallDown(wallIndex, progress, true); // Опускаем
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Завершаем движение
        mainCamera.transform.position = cameraPositions[newView];
        mainCamera.transform.rotation = targetCamRot;

        // Устанавливаем финальные позиции
        foreach (int wallIndex in wallsToHide)
        {
            SetWallFinalPosition(wallIndex, false);
        }

        foreach (int wallIndex in wallsToShow)
        {
            SetWallFinalPosition(wallIndex, true);
        }

        currentView = newView;
        isMoving = false;
    }

    void MoveWallDown(int wallIndex, float progress, bool moveDown)
    {
        Transform wall = GetWallByIndex(wallIndex);
        if (wall == null) return;

        Vector3 targetPosition = moveDown ? wallDownPositions[wallIndex] : upPosition;
        Vector3 startPosition = moveDown ? upPosition : wallDownPositions[wallIndex];

        wall.position = Vector3.Lerp(startPosition, targetPosition, progress);

        // Двигаем объекты на этой стене
        Transform[] objects = GetObjectsForWall(wallIndex);
        foreach (Transform obj in objects)
        {
            if (obj != null)
            {
                obj.position = Vector3.Lerp(startPosition, targetPosition, progress);
            }
        }
    }

    void SetWallFinalPosition(int wallIndex, bool isDown)
    {
        Transform wall = GetWallByIndex(wallIndex);
        if (wall == null) return;

        wall.position = isDown ? wallDownPositions[wallIndex] : upPosition;

        Transform[] objects = GetObjectsForWall(wallIndex);
        foreach (Transform obj in objects)
        {
            if (obj != null)
            {
                obj.position = isDown ? wallDownPositions[wallIndex] : upPosition;
            }
        }
    }

    void ShowView(int view)
    {
        int[] wallsToShow = GetWallsToShow(view);

        foreach (int wallIndex in wallsToShow)
        {
            Transform wall = GetWallByIndex(wallIndex);
            if (wall != null)
            {
                wall.position = wallDownPositions[wallIndex];
            }

            Transform[] objects = GetObjectsForWall(wallIndex);
            foreach (Transform obj in objects)
            {
                if (obj != null)
                {
                    obj.position = wallDownPositions[wallIndex];
                }
            }
        }
    }

    void HideAllObjects()
    {
        for (int i = 0; i < 4; i++)
        {
            Transform[] objects = GetObjectsForWall(i);
            foreach (Transform obj in objects)
            {
                if (obj != null)
                {
                    obj.position = upPosition;
                }
            }
        }
    }

    Transform GetWallByIndex(int index)
    {
        switch (index)
        {
            case 0: return wallNorth;
            case 1: return wallWest;
            case 2: return wallSouth;
            case 3: return wallEast;
            default: return null;
        }
    }

    Transform[] GetObjectsForWall(int wallIndex)
    {
        switch (wallIndex)
        {
            case 0: return northWallObjects;
            case 1: return westWallObjects;
            case 2: return southWallObjects;
            case 3: return eastWallObjects;
            default: return new Transform[0];
        }
    }

    int[] GetWallsToShow(int view)
    {
        switch (view)
        {
            case 0: return new int[] { 0, 1 }; // North + West
            case 1: return new int[] { 0, 3 }; // North + East
            case 2: return new int[] { 2, 3 }; // South + East
            case 3: return new int[] { 1, 2 }; // West + South
            default: return new int[0];
        }
    }

    int[] GetWallsToHide(int view)
    {
        switch (view)
        {
            case 0: return new int[] { 2, 3 }; // South + East
            case 1: return new int[] { 1, 2 }; // West + South
            case 2: return new int[] { 0, 1 }; // North + West
            case 3: return new int[] { 0, 3 }; // North + East
            default: return new int[0];
        }
    }
}