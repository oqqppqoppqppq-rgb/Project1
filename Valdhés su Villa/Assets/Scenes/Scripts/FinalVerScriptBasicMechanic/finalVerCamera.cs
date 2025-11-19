using System.Collections;
using UnityEngine;

public class finalVerCamera : MonoBehaviour
{
    public Vector3[] pos = new Vector3[4];
    public Vector3[] rot = new Vector3[4];
    public float speed = 0.5f;
    private bool moving = false;
    private int current = 0;

    void Update()
    {
        if (moving) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(current - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(current + 1);
        if (Input.GetKeyDown(KeyCode.Alpha1)) Move(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Move(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Move(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Move(3);
    }

    void Move(int num)
    {
        if (num < 0) num = 3;
        if (num > 3) num = 0;
        if (moving) return;

        StartCoroutine(MoveCam(num));
    }

    IEnumerator MoveCam(int num)
    {
        moving = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(rot[num]);

        float time = 0f;
        while (time < speed)
        {
            float t = time / speed;

            transform.position = Vector3.Lerp(startPos, pos[num], t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = pos[num];
        transform.rotation = endRot;
        current = num;
        moving = false;
    }
}