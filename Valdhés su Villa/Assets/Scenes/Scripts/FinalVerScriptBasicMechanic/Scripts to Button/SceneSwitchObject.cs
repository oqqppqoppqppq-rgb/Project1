using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchObject : MonoBehaviour
{
    public int sceneID = 1;

    void Start()
    {
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();
    }

    public void SwitchToScene()
    {
        SceneManager.LoadScene(sceneID);
    }
}