using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour
{
    [Header("Walls Setup")]
    public Transform[] walls = new Transform[4]; // 0:N, 1:W, 2:S, 3:E

    [Header("Settings")]
    public float moveSpeed = 1f;

    private Vector3 upPosition = new Vector3(0, 100, 0);
    private Vector3[] wallDownPositions = new Vector3[4];
    private Coroutine activeCoroutine;

    void Start()
    {
        for (int i = 0; i < 4; i++)
            if (walls[i] != null) wallDownPositions[i] = walls[i].position;

        SetupInitialState();
    }

    void SetupInitialState()
    {
        // Поднимаем все стены
        foreach (Transform wall in walls)
            if (wall != null) wall.position = upPosition;

        // Показываем стены для вида 0
        ShowWallsForView(0);
    }

    public void SwitchToView(int newView)
    {
        if (activeCoroutine != null) return;
        activeCoroutine = StartCoroutine(ChangeWalls(newView));
    }

    IEnumerator ChangeWalls(int view)
    {
        int[] wallsToShow = GetWallsToShow(view);
        int[] wallsToHide = GetWallsToHide(view);

        float elapsed = 0f;

        while (elapsed < moveSpeed)
        {
            float t = elapsed / moveSpeed;

            // Плавно скрываем стены
            foreach (int wallIndex in wallsToHide)
                MoveWall(wallIndex, t, false);

            // Плавно показываем стены
            foreach (int wallIndex in wallsToShow)
                MoveWall(wallIndex, t, true);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем финальные позиции
        foreach (int wallIndex in wallsToHide)
            SetWallPosition(wallIndex, false);

        foreach (int wallIndex in wallsToShow)
            SetWallPosition(wallIndex, true);

        activeCoroutine = null;
    }

    void MoveWall(int wallIndex, float t, bool show)
    {
        if (walls[wallIndex] == null) return;

        Vector3 startPos = show ? upPosition : wallDownPositions[wallIndex];
        Vector3 targetPos = show ? wallDownPositions[wallIndex] : upPosition;

        walls[wallIndex].position = Vector3.Lerp(startPos, targetPos, t);
    }

    void SetWallPosition(int wallIndex, bool show)
    {
        if (walls[wallIndex] == null) return;
        walls[wallIndex].position = show ? wallDownPositions[wallIndex] : upPosition;
    }

    void ShowWallsForView(int view)
    {
        int[] wallsToShow = GetWallsToShow(view);
        foreach (int wallIndex in wallsToShow)
            SetWallPosition(wallIndex, true);
    }

    int[] GetWallsToShow(int view)
    {
        return view switch
        {
            0 => new int[] { 0, 1 }, // Север + Запад
            1 => new int[] { 0, 3 }, // Север + Восток
            2 => new int[] { 2, 3 }, // Юг + Восток
            3 => new int[] { 1, 2 }, // Запад + Юг
            _ => new int[0]
        };
    }

    int[] GetWallsToHide(int view)
    {
        return view switch
        {
            0 => new int[] { 2, 3 }, // Юг + Восток
            1 => new int[] { 1, 2 }, // Запад + Юг
            2 => new int[] { 0, 1 }, // Север + Запад
            3 => new int[] { 0, 3 }, // Север + Восток
            _ => new int[0]
        };
    }
}