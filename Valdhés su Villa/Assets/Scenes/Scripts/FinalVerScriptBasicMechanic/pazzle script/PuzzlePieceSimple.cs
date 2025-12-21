using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public int correctPos;
    public int currentPos;

    void Start()
    {
        currentPos = correctPos;
    }

    void OnMouseDown()
    {
        // Просто меняем с любым кусочком при клике
        PuzzlePiece[] allPieces = FindObjectsOfType<PuzzlePiece>();

        foreach (PuzzlePiece other in allPieces)
        {
            if (other != this)
            {
                SwapWith(other);
                break;
            }
        }
    }

    void SwapWith(PuzzlePiece other)
    {
        // Меняем позиции
        Vector3 myPos = transform.position;
        transform.position = other.transform.position;
        other.transform.position = myPos;

        // Меняем currentPos
        int temp = currentPos;
        currentPos = other.currentPos;
        other.currentPos = temp;

        Debug.Log($"Поменяли {name} ({currentPos}) с {other.name} ({other.currentPos})");
    }
}