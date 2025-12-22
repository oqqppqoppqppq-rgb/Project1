using UnityEngine;
using UnityEngine.SceneManagement;

public class SafePuzzle : MonoBehaviour
{
    public SafeCircle circle1, circle2, circle3;
    public AudioClip wrongSound, openSound;
    public Animator safeAnimator;
    public int finalScene = 7; // Номер финальной сцены

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
            OpenSafe();
        }
        else
        {
            PlayWrongSound();
        }
    }

    void OpenSafe()
    {
        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position);

        if (safeAnimator != null)
            safeAnimator.SetTrigger("Open");

        // Задержка перед загрузкой финальной сцены
        Invoke("LoadFinalScene", 2f);
    }

    void LoadFinalScene()
    {
        SceneManager.LoadScene(finalScene);
    }

    void PlayWrongSound()
    {
        if (wrongSound != null)
            AudioSource.PlayClipAtPoint(wrongSound, transform.position);
    }
}