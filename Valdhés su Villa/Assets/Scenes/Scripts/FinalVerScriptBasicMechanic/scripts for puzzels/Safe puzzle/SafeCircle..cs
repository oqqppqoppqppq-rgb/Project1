using UnityEngine;

public class SafeCircle : MonoBehaviour
{
    public int currentPosition = 0;
    public int correctPosition = 0;
    public AudioClip clickSound;

    private float startRotationX = -90f; // Начальный поворот

    void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        // Устанавливаем начальный поворот
        Vector3 rot = transform.localEulerAngles;
        rot.x = startRotationX;
        transform.localEulerAngles = rot;
    }

    void OnMouseDown()
    {
        currentPosition++;
        if (currentPosition > 7) currentPosition = 0;

        // Вращаем от начального положения -90°
        float newRotationX = startRotationX + (currentPosition * 45f);

        Vector3 currentRotation = transform.localEulerAngles;
        currentRotation.x = newRotationX;
        transform.localEulerAngles = currentRotation;

        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
    }
}