using UnityEngine;

public class ButtonPuzzleManager : MonoBehaviour
{
    public AudioClip wrongSound;
    public AudioClip winSound;
    public GameObject movingObject;
    public Vector3 targetPosition;
    public float moveSpeed = 5f;

    private int[] correctSequence = { 1, 4, 6, 3 }; // Кнопки 1-4-6-3
    private int[] playerSequence = new int[4];
    private int currentStep = 0;
    private bool puzzleSolved = false;

    public void ButtonPressed(int buttonNumber)
    {
        if (puzzleSolved) return;

        playerSequence[currentStep] = buttonNumber;
        currentStep++;

        if (currentStep == 4)
        {
            CheckSequence();
        }
    }

    void CheckSequence()
    {
        bool correct = true;

        for (int i = 0; i < 4; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            puzzleSolved = true;
            if (winSound != null)
                AudioSource.PlayClipAtPoint(winSound, transform.position);

            StartCoroutine(MoveObject());
        }
        else
        {
            if (wrongSound != null)
                AudioSource.PlayClipAtPoint(wrongSound, transform.position);

            currentStep = 0;
        }
    }

    System.Collections.IEnumerator MoveObject()
    {
        while (Vector3.Distance(movingObject.transform.position, targetPosition) > 0.1f)
        {
            movingObject.transform.position = Vector3.MoveTowards(
                movingObject.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}