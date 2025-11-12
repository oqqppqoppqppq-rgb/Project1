using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZOOMscript : MonoBehaviour
{
    public float speed = 5f, zoomSpeed = 5, leftRightMin = -10f, leftRightMax = 10f, forwardBackMin = -10f, forwardBackMax = 10f, zoomMin = 2f, zoomMax = 20f;

// Все переменные вставлены одной строчкой, чтобы не зацикливать внимание на их корректности (мешает при редакции кода + не заполняет его постоянными строками 'Public void ...')

    void Update()
    {
        MoveCamera();
        ZoomCamera();
        MaxSizes();
    }
    void MoveCamera()
    {
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W)) moveZ += 1; // шаблон
        if (Input.GetKey(KeyCode.S)) moveZ -= 1;
        if (Input.GetKey(KeyCode.A)) moveX -= 1;
        if (Input.GetKey(KeyCode.D)) moveX += 1;

        transform.Translate(new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime); // шаблон
    }

    void ZoomCamera()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0) // если функция работает
        {
            transform.Translate(0, 0, scroll * zoomSpeed); // шаблон
        }
    }

    void MaxSizes() // Интеграция настройки сдвига камеры по x и z 
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, leftRightMin, leftRightMax); // Clamp - функция для настройки max и min значений
        pos.z = Mathf.Clamp(pos.z, forwardBackMin, forwardBackMax);

        float zoom = Mathf.Abs(pos.z); 
        if (zoom < zoomMin) pos.z = zoomMin * Mathf.Sign(pos.z);
        if (zoom > zoomMax) pos.z = zoomMax * Mathf.Sign(pos.z);

        transform.position = pos;
    }
}