using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzlePiece[] pieces;
    public DropController dropController;
    private bool solved = false;

    void Start()
    {
        // Автоматически находим все кусочки
        pieces = FindObjectsOfType<PuzzlePiece>();
        Shuffle();
    }

    void Update()
    {
        if (!solved && IsCorrect())
        {
            solved = true;
            Debug.Log("✅ Пазл решён!");
            dropController.StartDrop();
        }
    }

    bool IsCorrect()
    {
        foreach (PuzzlePiece piece in pieces)
        {
            if (piece.currentPos != piece.correctPos)
                return false;
        }
        return true;
    }

    void Shuffle()
    {
        for (int i = 0; i < 10; i++)
        {
            int a = Random.Range(0, pieces.Length);
            int b = Random.Range(0, pieces.Length);

            if (a != b)
            {
                Vector3 posA = pieces[a].transform.position;
                pieces[a].transform.position = pieces[b].transform.position;
                pieces[b].transform.position = posA;

                int currentA = pieces[a].currentPos;
                pieces[a].currentPos = pieces[b].currentPos;
                pieces[b].currentPos = currentA;
            }
        }
    }
}