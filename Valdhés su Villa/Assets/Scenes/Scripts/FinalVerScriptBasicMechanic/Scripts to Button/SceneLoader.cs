using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int sceneID = 1;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneID);
    }
}