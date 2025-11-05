using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalRoomController : MonoBehaviour
{
    [Header("Room Objects")]
    public Transform mainCamera;
    public Transform floor;
    public Transform WallNorthTest;
    public Transform wwt;
    public Transform swt;
    public Transform wet;

    [Header("Dependent Objects")]
    public List<Transform> northWallObjects = new List<Transform>();
    public List<Transform> wwtObjects = new List<Transform>();
    public List<Transform> swtObjects = new List<Transform>();
    public List<Transform> wetObjects = new List<Transform>();

    [Header("Settings")]
    public float rotationDuration = 0.5f;

    private Vector3 hiddenPosition = new Vector3(0, 100f, 0);
    private int currentViewIndex = 0;
    private bool isRotating = false;
    private Dictionary<Transform, Vector3> objectLocalPositions = new Dictionary<Transform, Vector3>();

    // Правильные позиции камеры для 4 направлений
    private readonly Vector3[] cameraPositions = {
        new Vector3(3.105f, 2.872f, -4.943f),  // Вид 0: North-West
        new Vector3(-4.943f, 2.872f, -3.105f), // Вид 1: North-East  
        new Vector3(-3.105f, 2.872f, 4.943f),  // Вид 2: South-East
        new Vector3(4.943f, 2.872f, 3.105f)    // Вид 3: South-West
    };

    // Правильные повороты камеры для 4 направлений
    private readonly Vector3[] cameraRotations = {
        new Vector3(17f, 310f, 0f),  // Вид 0: North-West
        new Vector3(17f, 40f, 0f),   // Вид 1: North-East
        new Vector3(17f, 130f, 0f),  // Вид 2: South-East
        new Vector3(17f, 220f, 0f)   // Вид 3: South-West
    };

    private readonly WallConfig[] wallConfigs = {
        new WallConfig { visible1 = "WallNorthTest", visible2 = "wwt", hidden1 = "wet", hidden2 = "swt" },
        new WallConfig { visible1 = "WallNorthTest", visible2 = "wet", hidden1 = "wwt", hidden2 = "swt" },
        new WallConfig { visible1 = "swt", visible2 = "wet", hidden1 = "WallNorthTest", hidden2 = "wwt" },
        new WallConfig { visible1 = "swt", visible2 = "wwt", hidden1 = "WallNorthTest", hidden2 = "wet" }
    };

    [System.Serializable]
    private struct WallConfig
    {
        public string visible1;
        public string visible2;
        public string hidden1;
        public string hidden2;
    }

    void Start()
    {
        CacheObjectPositions();
        InitializeRoom();
    }

    void Update()
    {
        if (isRotating) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) RotateRoom(-1);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) RotateRoom(1);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SetView(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetView(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetView(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SetView(3);
    }

    void CacheObjectPositions()
    {
        CacheObjectsForWall(northWallObjects, WallNorthTest);
        CacheObjectsForWall(wwtObjects, wwt);
        CacheObjectsForWall(swtObjects, swt);
        CacheObjectsForWall(wetObjects, wet);
    }

    void CacheObjectsForWall(List<Transform> objects, Transform wall)
    {
        foreach (Transform obj in objects)
        {
            if (obj != null && wall != null)
            {
                objectLocalPositions[obj] = wall.InverseTransformPoint(obj.position);
            }
        }
    }

    void InitializeRoom()
    {
        SetWallPositions(0);
        mainCamera.position = cameraPositions[0];
        mainCamera.eulerAngles = cameraRotations[0];
    }

    public void RotateRoom(int direction)
    {
        if (isRotating) return;
        int newDirection = (currentViewIndex + direction + 4) % 4;
        StartCoroutine(RotateRoomCoroutine(newDirection));
    }

    public void SetView(int direction)
    {
        if (direction < 0 || direction > 3 || isRotating) return;
        StartCoroutine(RotateRoomCoroutine(direction));
    }
    private IEnumerator RotateRoomCoroutine(int newDirection)
    {
        isRotating = true;

        var currentWalls = wallConfigs[currentViewIndex];
        var newWalls = wallConfigs[newDirection];

        Transform wallToRaise = GetWallByName(GetExitingWall(currentWalls, newWalls));
        Transform wallToLower = GetWallByName(GetEnteringWall(currentWalls, newWalls));

        if (wallToRaise != null && wallToLower != null)
        {
            yield return StartCoroutine(AnimateWalls(wallToRaise, wallToLower));
        }

        yield return StartCoroutine(AnimateCamera(newDirection));

        currentViewIndex = newDirection;
        isRotating = false;
    }

    private string GetExitingWall(WallConfig current, WallConfig next)
    {
        if (current.visible1 != next.visible1 && current.visible1 != next.visible2) return current.visible1;
        if (current.visible2 != next.visible1 && current.visible2 != next.visible2) return current.visible2;
        return null;
    }

    private string GetEnteringWall(WallConfig current, WallConfig next)
    {
        if (next.visible1 != current.visible1 && next.visible1 != current.visible2) return next.visible1;
        if (next.visible2 != current.visible1 && next.visible2 != current.visible2) return next.visible2;
        return null;
    }

    private Transform GetWallByName(string wallName)
    {
        switch (wallName)
        {
            case "WallNorthTest": return WallNorthTest;
            case "wwt": return wwt;
            case "swt": return swt;
            case "wet": return wet;
            default: return null;
        }
    }

    private List<Transform> GetObjectsForWall(Transform wall)
    {
        if (wall == WallNorthTest) return northWallObjects;
        if (wall == wwt) return wwtObjects;
        if (wall == swt) return swtObjects;
        if (wall == wet) return wetObjects;
        return new List<Transform>();
    }

    private IEnumerator AnimateWalls(Transform wallToRaise, Transform wallToLower)
    {
        float elapsedTime = 0f;
        Vector3 raisedPosition = new Vector3(wallToRaise.position.x, hiddenPosition.y, wallToRaise.position.z);
        Vector3 loweredPosition = GetWallTargetPosition(wallToLower);

        Vector3 startRaisePos = wallToRaise.position;
        Vector3 startLowerPos = wallToLower.position;

        List<Transform> raiseObjects = GetObjectsForWall(wallToRaise);
        List<Transform> lowerObjects = GetObjectsForWall(wallToLower);

        Dictionary<Transform, Vector3> raiseObjectStartPos = new Dictionary<Transform, Vector3>();
        Dictionary<Transform, Vector3> lowerObjectStartPos = new Dictionary<Transform, Vector3>();

        foreach (Transform obj in raiseObjects)
        {
            if (obj != null) raiseObjectStartPos[obj] = obj.position;
        }
        foreach (Transform obj in lowerObjects)
        {
            if (obj != null) lowerObjectStartPos[obj] = obj.position;
        }

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            wallToRaise.position = Vector3.Lerp(startRaisePos, raisedPosition, smoothT);
            wallToLower.position = Vector3.Lerp(startLowerPos, loweredPosition, smoothT);

            foreach (Transform obj in raiseObjects)
            {
                if (obj != null && raiseObjectStartPos.ContainsKey(obj))
                {
                    Vector3 targetPos = new Vector3(obj.position.x, hiddenPosition.y, obj.position.z);
                    obj.position = Vector3.Lerp(raiseObjectStartPos[obj], targetPos, smoothT);
                }
            }
            foreach (Transform obj in lowerObjects)
            {
                if (obj != null && lowerObjectStartPos.ContainsKey(obj) && objectLocalPositions.ContainsKey(obj))
                {
                    Vector3 targetWorldPos = wallToLower.TransformPoint(objectLocalPositions[obj]);
                    obj.position = Vector3.Lerp(lowerObjectStartPos[obj], targetWorldPos, smoothT);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wallToRaise.position = raisedPosition;
        wallToLower.position = loweredPosition;

        foreach (Transform obj in lowerObjects)
        {
            if (obj != null && objectLocalPositions.ContainsKey(obj))
            {
                obj.position = wallToLower.TransformPoint(objectLocalPositions[obj]);
            }
        }
    }

    private IEnumerator AnimateCamera(int newDirection)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.position;
        Vector3 targetPos = cameraPositions[newDirection];
        Vector3 startRot = mainCamera.eulerAngles;
        Vector3 targetRot = cameraRotations[newDirection];

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.position = Vector3.Lerp(startPos, targetPos, smoothT);

            Vector3 currentRot = new Vector3(
                Mathf.LerpAngle(startRot.x, targetRot.x, smoothT),
                Mathf.LerpAngle(startRot.y, targetRot.y, smoothT),
                Mathf.LerpAngle(startRot.z, targetRot.z, smoothT)
            );
            mainCamera.eulerAngles = currentRot;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.position = targetPos;
        mainCamera.eulerAngles = targetRot;
    }

    private Vector3 GetWallTargetPosition(Transform wall)
    {
        if (wall == WallNorthTest) return new Vector3(-1.908f, 0.015f, 2.108f);
        if (wall == wwt) return new Vector3(-2f, 0f, -3f);
        if (wall == swt) return new Vector3(2.445f, 0f, -3f);
        if (wall == wet) return new Vector3(2f, 0f, 1f);
        return Vector3.zero;
    }

    private void SetWallPositions(int direction)
    {
        var walls = wallConfigs[direction];

        GetWallByName(walls.visible1).position = GetWallTargetPosition(GetWallByName(walls.visible1));
        GetWallByName(walls.visible2).position = GetWallTargetPosition(GetWallByName(walls.visible2));
        GetWallByName(walls.hidden1).position = hiddenPosition;
        GetWallByName(walls.hidden2).position = hiddenPosition;

        UpdateObjectsPosition(GetWallByName(walls.visible1), GetObjectsForWall(GetWallByName(walls.visible1)));
        UpdateObjectsPosition(GetWallByName(walls.visible2), GetObjectsForWall(GetWallByName(walls.visible2)));
    }

    private void UpdateObjectsPosition(Transform wall, List<Transform> objects)
    {
        foreach (Transform obj in objects)
        {
            if (obj != null && objectLocalPositions.ContainsKey(obj))
            {
                obj.position = wall.TransformPoint(objectLocalPositions[obj]);
            }
        }
    }

    public void AddObjectToNorthWall(Transform obj) => AddObjectToWall(obj, WallNorthTest, northWallObjects);
    public void AddObjectToWwt(Transform obj) => AddObjectToWall(obj, wwt, wwtObjects);
    public void AddObjectToSwt(Transform obj) => AddObjectToWall(obj, swt, swtObjects);
    public void AddObjectToWet(Transform obj) => AddObjectToWall(obj, wet, wetObjects);

    private void AddObjectToWall(Transform obj, Transform wall, List<Transform> objectList)
    {
        if (!objectList.Contains(obj))
        {
            objectList.Add(obj);
            objectLocalPositions[obj] = wall.InverseTransformPoint(obj.position);
        }
    }
}