using UnityEngine;

public class OpenButton : MonoBehaviour
{
    public SafePuzzle safePuzzle;
    public AudioClip buttonSound;

    void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    void OnMouseDown()
    {
        if (buttonSound != null)
            AudioSource.PlayClipAtPoint(buttonSound, transform.position);

        if (safePuzzle != null)
            safePuzzle.CheckCombination();
    }
}