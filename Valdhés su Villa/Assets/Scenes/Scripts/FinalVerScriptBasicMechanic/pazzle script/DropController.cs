using UnityEngine;

public class DropController : MonoBehaviour
{
    public GameObject obj;
    public Vector3 target;
    public float speed = 10f;

    private bool dropping = false;

    void Start()
    {
        obj.transform.position += new Vector3(0, 100, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            dropping = true;
        }

        if (dropping)
        {
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position, target, speed * Time.deltaTime);
        }
    }

    public void StartDrop()
    {
        dropping = true;
    }
}