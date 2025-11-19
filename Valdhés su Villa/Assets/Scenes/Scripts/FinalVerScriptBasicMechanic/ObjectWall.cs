using System.Collections.Generic;
using UnityEngine;

public class WallObjects : MonoBehaviour
{
    public WallController wallController;
    public List <Transform> objectsForWall0 = new List <Transform>();
    public List <Transform> objectsForWall1 = new List <Transform>();
    public List <Transform> objectsForWall2 = new List <Transform>();
    public List <Transform> objectsForWall3 = new List <Transform>();

    void Start()
    {
        AddObjectsToWalls();
    }

    void AddObjectsToWalls()
    {
        foreach (Transform obj in objectsForWall0)
            wallController.AddObject(obj, 0);
        foreach (Transform obj in objectsForWall1)
            wallController.AddObject(obj, 1);
        foreach (Transform obj in objectsForWall2)
            wallController.AddObject(obj, 2);
        foreach (Transform obj in objectsForWall3)
            wallController.AddObject(obj, 3);
    }

    public void AddNewObject(Transform obj, int wallIndex)
    {
        switch (wallIndex)
        {
            case 0: objectsForWall0.Add(obj); break;
            case 1: objectsForWall1.Add(obj); break;
            case 2: objectsForWall2.Add(obj); break;
            case 3: objectsForWall3.Add(obj); break;
        }
        wallController.AddObject(obj, wallIndex);
    }
}