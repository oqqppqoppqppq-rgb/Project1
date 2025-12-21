using UnityEngine;

public class SafePuzzle : MonoBehaviour
{
    public SafeCircle circle1, circle2, circle3;
    public AudioClip wrongSound, openSound;
    public Animator safeAnimator;

    void Start()
    {
        circle1.correctPosition = 1;
        circle2.correctPosition = 6;
        circle3.correctPosition = 4;
    }

    public void CheckCombination()
    {
        bool correct = circle1.currentPosition == circle1.correctPosition &&
                      circle2.currentPosition == circle2.correctPosition &&
                      circle3.currentPosition == circle3.correctPosition;

        if (correct)
        {
            AudioSource.PlayClipAtPoint(openSound, transform.position);
            if (safeAnimator) safeAnimator.SetTrigger("Open");
        }
        else
        {
            AudioSource.PlayClipAtPoint(wrongSound, transform.position);
        }
    }
}