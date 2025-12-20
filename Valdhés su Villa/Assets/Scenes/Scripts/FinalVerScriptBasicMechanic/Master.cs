using UnityEngine;

public class MasterController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchAllControllers(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchAllControllers(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchAllControllers(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchAllControllers(3);
    }

    void SwitchAllControllers(int view)
    {
        // Находим все контроллеры в сцене
        CameraController camera = FindObjectOfType<CameraController>();
        WallController walls = FindObjectOfType<WallController>();
        ObjectController objects = FindObjectOfType<ObjectController>();

        // Запускаем анимации на всех контроллерах
        if (camera != null) camera.MoveToView(view);
        if (walls != null) walls.SwitchToView(view);
        if (objects != null) objects.SwitchToView(view);
    }
}