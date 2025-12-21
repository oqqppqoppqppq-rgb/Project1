using UnityEngine;

public class ButtonPuzzle : MonoBehaviour
{
    public int buttonNumber; // 1,2,3,4,5,6
    public AudioClip buttonSound;
    public ButtonPuzzleManager puzzleManager;

    void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    void OnMouseDown()
    {
        if (buttonSound != null)
            AudioSource.PlayClipAtPoint(buttonSound, transform.position);

        puzzleManager.ButtonPressed(buttonNumber);
    }
}