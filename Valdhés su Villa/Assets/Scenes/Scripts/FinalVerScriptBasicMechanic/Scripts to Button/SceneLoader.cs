using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int sceneID = 1;

    void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    void OnMouseDown()
    {
        if (sceneID >= 0 && sceneID < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneID);
        }
    }
}