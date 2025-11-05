using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest1 : MonoBehaviour
{
    [Header("Camera Settings")]
    public float rotationDuration = 0.5f;

    [Header("Wall References")]
    public Transform awEastTEST, awWestTEST, awSouthTEST, awNorthTEST, floor;

    [Header("Object Management")]
    public List<Transform> northWallObjects = new List<Transform>();
    public List<Transform> southWallObjects = new List<Transform>();
    public List<Transform> eastWallObjects = new List<Transform>();
    public List<Transform> westWallObjects = new List<Transform>();

    [Header("Wall Positions")]
    public Vector3 wallHiddenPosition = new Vector3(0, 10f, 0);

    public enum RoomView { NorthEast = 0, SouthEast = 1, SouthWest = 2, NorthWest = 3 }

    private RoomView currentView = RoomView.NorthEast;
    private bool isRotating = false;
    private Dictionary<Transform, Vector3> objectLocalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Vector3> objectLocalRotations = new Dictionary<Transform, Vector3>();

    private readonly Vector3[] cameraPositions = {
        new Vector3(5.69f, 6.14f, -7.56f), new Vector3(5.69f, 6.14f, 7.56f),
        new Vector3(-5.69f, 6.14f, 7.56f), new Vector3(-5.69f, 6.14f, -7.56f)
    };

    private readonly Quaternion[] cameraRotations = {
        Quaternion.Euler(24.408f, -53.281f, 0.009f), Quaternion.Euler(24.408f, -126.719f, 0.009f),
        Quaternion.Euler(24.408f, 126.719f, 0.009f), Quaternion.Euler(24.408f, 53.281f, 0.009f)
    };

    void Start()
    {
        CacheObjectPositions();
        UpdateWallsForCurrentView();
        UpdateObjectsForCurrentView();
    }

    void Update()
    {
        if (isRotating) return;
        HandleInput();
    }

    void CacheObjectPositions()
    {
        CacheObjectsForWall(northWallObjects, awNorthTEST);
        CacheObjectsForWall(southWallObjects, awSouthTEST);
        CacheObjectsForWall(eastWallObjects, awEastTEST);
        CacheObjectsForWall(westWallObjects, awWestTEST);
    }

    void CacheObjectsForWall(List<Transform> objects, Transform wall)
    {
        foreach (Transform obj in objects)
        {
            if (obj != null)
            {
                objectLocalPositions[obj] = wall.InverseTransformPoint(obj.position);
                objectLocalRotations[obj] = (Quaternion.Inverse(wall.rotation) * obj.rotation).eulerAngles;
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToView(RoomView.NorthEast);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToView(RoomView.SouthEast);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToView(RoomView.SouthWest);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchToView(RoomView.NorthWest);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) RotateRoomLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) RotateRoomRight();
    }

    public void SwitchToView(RoomView targetView)
    {
        if (isRotating || currentView == targetView) return;
        StartCoroutine(SwitchToViewCoroutine(targetView));
    }

    IEnumerator SwitchToViewCoroutine(RoomView targetView)
    {
        isRotating = true;

        Transform wallToStay = GetStayingWall(currentView, targetView);
        Transform wallToExit = GetExitingWall(currentView, targetView);
        Transform wallToEnter = GetEnteringWall(currentView, targetView);

        if (wallToExit != null && wallToEnter != null)
        {
            StartCoroutine(AnimateWallTransition(wallToExit, wallToEnter,
                GetObjectsForWall(wallToExit), GetObjectsForWall(wallToEnter)));
        }

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 targetPos = cameraPositions[(int)targetView];
        Quaternion targetRot = cameraRotations[(int)targetView];
        float elapsed = 0f;
        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotationDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        currentView = targetView;
        isRotating = false;
    }

    IEnumerator AnimateWallTransition(Transform wallToExit, Transform wallToEnter,
                                    List<Transform> objectsToExit, List<Transform> objectsToEnter)
    {
        float elapsed = 0f;
        Vector3 exitStartPos = wallToExit.position;
        Vector3 enterStartPos = wallHiddenPosition;
        Vector3 enterTargetPos = GetWallPosition(wallToEnter);

        Dictionary<Transform, Vector3> exitObjectStartPositions = new Dictionary<Transform, Vector3>();
        Dictionary<Transform, Vector3> enterObjectStartPositions = new Dictionary<Transform, Vector3>();

        foreach (Transform obj in objectsToExit) if (obj != null) exitObjectStartPositions[obj] = obj.position;
        foreach (Transform obj in objectsToEnter) if (obj != null) enterObjectStartPositions[obj] = wallHiddenPosition;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / rotationDuration);

            wallToExit.position = Vector3.Lerp(exitStartPos, wallHiddenPosition, t);
            wallToEnter.position = Vector3.Lerp(enterStartPos, enterTargetPos, t);

            foreach (Transform obj in objectsToExit)
                if (obj != null && exitObjectStartPositions.ContainsKey(obj))
                    obj.position = Vector3.Lerp(exitObjectStartPositions[obj], wallHiddenPosition, t);

            foreach (Transform obj in objectsToEnter)
                if (obj != null && enterObjectStartPositions.ContainsKey(obj))
                    AnimateObjectTransition(obj, wallToEnter, enterObjectStartPositions[obj], t);

            yield return null;
        }

        wallToExit.position = wallHiddenPosition;
        wallToEnter.position = enterTargetPos;
        UpdateObjectsPosition(wallToEnter, objectsToEnter);
    }

    void AnimateObjectTransition(Transform obj, Transform wall, Vector3 startPos, float t)
    {
        Vector3 targetWorldPos = wall.TransformPoint(objectLocalPositions[obj]);
        obj.position = Vector3.Lerp(startPos, targetWorldPos, t);
        Vector3 targetRotation = objectLocalRotations[obj];
        obj.rotation = Quaternion.Lerp(Quaternion.Euler(startPos), wall.rotation * Quaternion.Euler(targetRotation), t);
    }

    void UpdateObjectsPosition(Transform wall, List<Transform> objects)
    {
        foreach (Transform obj in objects)
        {
            if (obj != null && objectLocalPositions.ContainsKey(obj))
            {
                obj.position = wall.TransformPoint(objectLocalPositions[obj]);
                if (objectLocalRotations.ContainsKey(obj))
                    obj.rotation = wall.rotation * Quaternion.Euler(objectLocalRotations[obj]);
            }
        }
    }

    List<Transform> GetObjectsForWall(Transform wall)
    {
        if (wall == awNorthTEST) return northWallObjects;
        if (wall == awSouthTEST) return southWallObjects;
        if (wall == awEastTEST) return eastWallObjects;
        if (wall == awWestTEST) return westWallObjects;
        return new List<Transform>();
    }

    void UpdateObjectsForCurrentView()
    {
        switch (currentView)
        {
            case RoomView.NorthEast:
                UpdateObjectsPosition(awNorthTEST, northWallObjects);
                UpdateObjectsPosition(awEastTEST, eastWallObjects);
                break;
            case RoomView.SouthEast:
                UpdateObjectsPosition(awSouthTEST, southWallObjects);
                UpdateObjectsPosition(awEastTEST, eastWallObjects);
                break;
            case RoomView.SouthWest:
                UpdateObjectsPosition(awSouthTEST, southWallObjects);
                UpdateObjectsPosition(awWestTEST, westWallObjects);
                break;
            case RoomView.NorthWest:
                UpdateObjectsPosition(awNorthTEST, northWallObjects);
                UpdateObjectsPosition(awWestTEST, westWallObjects);
                break;
        }
        HideObjectsOnInvisibleWalls();
    }

    void HideObjectsOnInvisibleWalls()
    {
        List<Transform> invisibleWalls = GetInvisibleWalls();
        foreach (Transform wall in invisibleWalls)
        {
            List<Transform> objects = GetObjectsForWall(wall);
            foreach (Transform obj in objects)
                if (obj != null) obj.position = wallHiddenPosition;
        }
    }

    List<Transform> GetInvisibleWalls()
    {
        List<Transform> invisibleWalls = new List<Transform>();
        switch (currentView)
        {
            case RoomView.NorthEast: invisibleWalls.AddRange(new[] { awSouthTEST, awWestTEST }); break;
            case RoomView.SouthEast: invisibleWalls.AddRange(new[] { awNorthTEST, awWestTEST }); break;
            case RoomView.SouthWest: invisibleWalls.AddRange(new[] { awNorthTEST, awEastTEST }); break;
            case RoomView.NorthWest: invisibleWalls.AddRange(new[] { awSouthTEST, awEastTEST }); break;
        }
        return invisibleWalls;
    }

    void UpdateWallsForCurrentView()
    {
        awEastTEST.position = awWestTEST.position = awSouthTEST.position = awNorthTEST.position = wallHiddenPosition;

        switch (currentView)
        {
            case RoomView.NorthEast:
                awNorthTEST.position = GetWallPosition(awNorthTEST);
                awEastTEST.position = GetWallPosition(awEastTEST);
                break;
            case RoomView.SouthEast:
                awSouthTEST.position = GetWallPosition(awSouthTEST);
                awEastTEST.position = GetWallPosition(awEastTEST);
                break;
            case RoomView.SouthWest:
                awSouthTEST.position = GetWallPosition(awSouthTEST);
                awWestTEST.position = GetWallPosition(awWestTEST);
                break;
            case RoomView.NorthWest:
                awNorthTEST.position = GetWallPosition(awNorthTEST);
                awWestTEST.position = GetWallPosition(awWestTEST);
                break;
        }
    }

    Transform GetStayingWall(RoomView from, RoomView to)
    {
        if ((from == RoomView.NorthEast && to == RoomView.SouthEast) || (from == RoomView.SouthEast && to == RoomView.NorthEast)) return awEastTEST;
        if ((from == RoomView.SouthEast && to == RoomView.SouthWest) || (from == RoomView.SouthWest && to == RoomView.SouthEast)) return awSouthTEST;
        if ((from == RoomView.SouthWest && to == RoomView.NorthWest) || (from == RoomView.NorthWest && to == RoomView.SouthWest)) return awWestTEST;
        if ((from == RoomView.NorthWest && to == RoomView.NorthEast) || (from == RoomView.NorthEast && to == RoomView.NorthWest)) return awNorthTEST;
        return null;
    }

    Transform GetExitingWall(RoomView from, RoomView to)
    {
        switch (from)
        {
            case RoomView.NorthEast: return to == RoomView.SouthEast ? awNorthTEST : awEastTEST;
            case RoomView.SouthEast: return to == RoomView.SouthWest ? awEastTEST : awSouthTEST;
            case RoomView.SouthWest: return to == RoomView.NorthWest ? awSouthTEST : awWestTEST;
            case RoomView.NorthWest: return to == RoomView.NorthEast ? awWestTEST : awNorthTEST;
            default: return null;
        }
    }
    Transform GetEnteringWall(RoomView from, RoomView to)
    {
        switch (to)
        {
            case RoomView.NorthEast: return from == RoomView.SouthEast ? awNorthTEST : awEastTEST;
            case RoomView.SouthEast: return from == RoomView.SouthWest ? awEastTEST : awSouthTEST;
            case RoomView.SouthWest: return from == RoomView.NorthWest ? awSouthTEST : awWestTEST;
            case RoomView.NorthWest: return from == RoomView.NorthEast ? awWestTEST : awNorthTEST;
            default: return null;
        }
    }

    Vector3 GetWallPosition(Transform wall)
    {
        if (wall == awNorthTEST) return new Vector3(-0.02f, 1.5566f, 7.08f);
        if (wall == awSouthTEST) return new Vector3(-0.01654f, 1.5566f, -7.078f);
        if (wall == awEastTEST) return new Vector3(-4f, 1.5566f, 0.014f);
        if (wall == awWestTEST) return new Vector3(4f, 1.5566f, 0.014f);
        return Vector3.zero;
    }

    public void RotateRoomLeft() => RotateRoom(-1);
    public void RotateRoomRight() => RotateRoom(1);

    void RotateRoom(int direction)
    {
        if (isRotating) return;
        RoomView targetView = (RoomView)(((int)currentView + direction + 4) % 4);
        SwitchToView(targetView);
    }

    // UI Methods
    public void OnView1Button() => SwitchToView(RoomView.NorthEast);
    public void OnView2Button() => SwitchToView(RoomView.SouthEast);
    public void OnView3Button() => SwitchToView(RoomView.SouthWest);
    public void OnView4Button() => SwitchToView(RoomView.NorthWest);
    public void OnLeftRotationButton() => RotateRoomLeft();
    public void OnRightRotationButton() => RotateRoomRight();
    public void SetView(int viewIndex)
    {
        if (viewIndex >= 0 && viewIndex < 4 && !isRotating)
            SwitchToView((RoomView)viewIndex);
    }

    public void AddObjectToWall(Transform obj, Transform wall)
    {
        List<Transform> objectList = GetObjectsForWall(wall);
        if (!objectList.Contains(obj))
        {
            objectList.Add(obj);
            CacheObjectPosition(obj, wall);
        }
    }

    void CacheObjectPosition(Transform obj, Transform wall)
    {
        objectLocalPositions[obj] = wall.InverseTransformPoint(obj.position);
        objectLocalRotations[obj] = (Quaternion.Inverse(wall.rotation) * obj.rotation).eulerAngles;
    }
}