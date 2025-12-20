using UnityEngine;
using System.Collections.Generic;

public class SimpleRoomSystem : MonoBehaviour
{
    public Camera mainCamera;
    public Transform[] walls = new Transform[4];
    public List<Transform>[] wallObjects = new List<Transform>[4];
    public float moveTime = 0.5f;

    private bool moving = false;
    private int currentView = 0;
    private Vector3 hidePos = new Vector3(0, 100, 0);
    private Vector3[] wallStartPos = new Vector3[4];

    private int[,] views = { { 0, 1, 3, 2 }, { 0, 3, 1, 2 }, { 2, 3, 0, 1 }, { 2, 1, 3, 0 } };

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            wallObjects[i] = new List<Transform>();
            if (walls[i] != null) wallStartPos[i] = walls[i].position;
        }
    }

    void Update()
    {
        if (moving) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchView(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchView(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchView(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchView(3);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchView(currentView - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchView(currentView + 1);
    }

    void SwitchView(int newView)
    {
        if (newView < 0) newView = 3;
        if (newView > 3) newView = 0;
        if (moving) return;
        StartCoroutine(MoveEverything(newView));
    }

    System.Collections.IEnumerator MoveEverything(int newView)
    {
        moving = true;

        int hide1 = views[currentView, 2];
        int hide2 = views[currentView, 3];
        int show1 = views[newView, 0];
        int show2 = views[newView, 1];

        float t = 0f;
        while (t < moveTime)
        {
            float progress = t / moveTime;

            MoveWallAndObjects(hide1, progress, true);
            MoveWallAndObjects(hide2, progress, true);
            MoveWallAndObjects(show1, progress, false);
            MoveWallAndObjects(show2, progress, false);

            t += Time.deltaTime;
            yield return null;
        }

        FinalMove(hide1, true);
        FinalMove(hide2, true);
        FinalMove(show1, false);
        FinalMove(show2, false);

        currentView = newView;
        moving = false;
    }

    void MoveWallAndObjects(int wallIndex, float progress, bool hide)
    {
        if (walls[wallIndex] != null)
        {
            walls[wallIndex].position = hide
                ? Vector3.Lerp(wallStartPos[wallIndex], hidePos, progress)
                : Vector3.Lerp(hidePos, wallStartPos[wallIndex], progress);
        }

        foreach (Transform obj in wallObjects[wallIndex])
        {
            if (obj != null)
            {
                obj.position = hide
                    ? Vector3.Lerp(wallStartPos[wallIndex], hidePos, progress)
                    : Vector3.Lerp(hidePos, wallStartPos[wallIndex], progress);
            }
        }
    }

    void FinalMove(int wallIndex, bool hide)
    {
        if (walls[wallIndex] != null)
        {
            walls[wallIndex].position = hide ? hidePos : wallStartPos[wallIndex];
        }

        foreach (Transform obj in wallObjects[wallIndex])
        {
            if (obj != null)
            {
                obj.position = hide ? hidePos : wallStartPos[wallIndex];
            }
        }
    }

    public void AddObjectToWall(Transform obj, int wallIndex)
    {
        if (wallIndex >= 0 && wallIndex < 4)
            wallObjects[wallIndex].Add(obj);
    }
}