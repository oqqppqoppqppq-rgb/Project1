using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour

{
    public Transform[] walls = new Transform[4]; // Создание 4-х стен

    public List <Transform>[] wallObjects = new List <Transform>[4]; // Создание листа, для задачи объектов, принадлежащих определённой стене (будет двигаться вместе с ней)

    public float moveTime = 0.5f;

    private int currentView = 0;
    private bool Move = false;

    private Vector3 hiddenPos = new Vector3(0, 100f, 0);

    private Vector3[] wStartPos = new Vector3[4];

    private int[,] views = 
    {
        {0, 1, 3, 2}, // 0 - North, 1 - West, 2 - South, 3 - East
        {0, 3, 1, 2},
        {2, 3, 0, 1},
        {2, 1, 3, 0}
    };

    public void AddObject(Transform obj, int wallIndex)
    {

        if (wallIndex >= 0 && wallIndex < 4)
            wallObjects[wallIndex].Add(obj);

    }


    void Start()
    { 
        for(int i = 0; i < 4; i++)
        
        {
            wallObjects[i] = new List <Transform> ();

            if(walls[i] != null)
                wStartPos[i] = walls[i].position;
        }

        Walls(0);
    }

    void Update()

    {
        if (Move) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Walls(-1); // <-- binds
        else if (Input.GetKeyDown(KeyCode.RightArrow)) Walls(1); // -->
        else if (Input.GetKeyDown(KeyCode.Alpha1)) Walls(0); // 1
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Walls(1); // 2
        else if (Input.GetKeyDown(KeyCode.Alpha3)) Walls(2); // 3
        else if (Input.GetKeyDown(KeyCode.Alpha4)) Walls(3); // 4
    }

    void View(int direction)

    {

        int newView = (currentView + direction + 4) % 4; // Всего 4 вида
        StartCoroutine(MoveWalls(newView));

    }

    void Walls(int view)

    {

        if (view < 0 || view > 3 || Move) return;

        if (Move)

        {

            StopAllCoroutines();
            Move = false;

        }

        StartCoroutine(MoveWalls(view));
    }

    IEnumerator MoveWalls(int newView)

    {
        Move = true;

        int hide1 = views[currentView, 2];
        int hide2 = views[currentView, 3];
        int show1 = views[newView, 0];
        int show2 = views[newView, 1];

        float time = 0f;

        while (time < moveTime)
        {
            float t = time / moveTime;

            walls[hide1].position = Vector3.Lerp(walls[hide1].position, hiddenPos, t);
            walls[hide2].position = Vector3.Lerp(walls[hide2].position, hiddenPos, t);
            walls[show1].position = Vector3.Lerp(walls[show1].position, wStartPos[show1], t);
            walls[show2].position = Vector3.Lerp(walls[show2].position, wStartPos[show2], t);

            time += Time.deltaTime;
            yield return null;
        }

        walls[hide1].position = hiddenPos;
        walls[hide2].position = hiddenPos;
        walls[show1].position = wStartPos[show1];
        walls[show2].position = wStartPos[show2];

        currentView = newView;
        Move = false;
    }
}