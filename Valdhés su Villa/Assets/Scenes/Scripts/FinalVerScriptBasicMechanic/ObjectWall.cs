using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour
{
    [System.Serializable]
    public class WallData
    {
        public List<Transform> objects;
        [HideInInspector] public List<Vector3> startPositions;
    }

    public WallData[] walls = new WallData[4];
    public float moveSpeed = 1f;
    private Vector3 upPos = new Vector3(0, 100, 0);
    private Coroutine animCoroutine;

    void Start()
    {
        for (int w = 0; w < 4; w++)
        {
            walls[w].startPositions = new List<Vector3>();
            foreach (var obj in walls[w].objects)
                if (obj) walls[w].startPositions.Add(obj.position);
        }

        ShowView(0);
    }

    public void SwitchToView(int view) => StartAnimation(view);

    void StartAnimation(int view)
    {
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        animCoroutine = StartCoroutine(AnimateObjects(view));
    }

    IEnumerator AnimateObjects(int view)
    {
        int[] show = GetWallsToShow(view);
        int[] hide = GetWallsToHide(view);

        for (float t = 0; t < 1; t += Time.deltaTime / moveSpeed)
        {
            foreach (int i in hide) MoveWallObjects(i, t, false);
            foreach (int i in show) MoveWallObjects(i, t, true);
            yield return null;
        }

        foreach (int i in hide) SetWallObjects(i, false);
        foreach (int i in show) SetWallObjects(i, true);
    }

    void MoveWallObjects(int wallIndex, float t, bool down)
    {
        var wall = walls[wallIndex];
        for (int i = 0; i < wall.objects.Count; i++)
        {
            if (wall.objects[i])
            {
                Vector3 start = down ? upPos : wall.startPositions[i];
                Vector3 end = down ? wall.startPositions[i] : upPos;
                wall.objects[i].position = Vector3.Lerp(start, end, EaseInOut(t));
            }
        }
    }

    void SetWallObjects(int wallIndex, bool down)
    {
        var wall = walls[wallIndex];
        for (int i = 0; i < wall.objects.Count; i++)
            if (wall.objects[i])
                wall.objects[i].position = down ? wall.startPositions[i] : upPos;
    }

    void ShowView(int view)
    {
        foreach (int i in GetWallsToShow(view))
            SetWallObjects(i, true);
    }

    float EaseInOut(float t) => t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

    int[] GetWallsToShow(int view) => view switch
    {
        0 => new[] { 0, 1 },
        1 => new[] { 0, 3 },
        2 => new[] { 2, 3 },
        3 => new[] { 1, 2 },
        _ => new int[0]
    };

    int[] GetWallsToHide(int view) => view switch
    {
        0 => new[] { 2, 3 },
        1 => new[] { 1, 2 },
        2 => new[] { 0, 1 },
        3 => new[] { 0, 3 },
        _ => new int[0]
    };
}