using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    public int sceneID = 1; // 0=меню, 1=игра и т.д.
    public bool useDelay = false;
    public float delayTime = 0.5f;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button) button.onClick.AddListener(Load);
    }

    public void Load()
    {
        if (useDelay) Invoke("LoadNow", delayTime);
        else LoadNow();
    }

    void LoadNow()
    {
        if (sceneID >= 0 && sceneID < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(sceneID);
        else
            Debug.LogError($"Неверный ID сцены: {sceneID}");
    }

#if UNITY_EDITOR
    void OnValidate() => gameObject.name = $"Btn_Scene{sceneID}";
#endif
}