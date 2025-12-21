using UnityEngine;

public class Phone : MonoBehaviour
{
    public AudioClip sound;

    void OnMouseDown()
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }
}