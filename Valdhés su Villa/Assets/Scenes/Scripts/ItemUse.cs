using UnityEngine;
using static UnityEditor.Progress;

public class ItemUse : MonoBehaviour
{
    public string itemName;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Inv.Add(itemName);
            gameObject.SetActive(false);
        }
    }
}

public class Inv : MonoBehaviour
{
    public static Transform panel;

    public static void Add(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(panel);
    }

    public static void Remove(string name)
    {
        foreach (Transform child in panel)
        {
            if (child.name == name)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
}

public class UseItem : MonoBehaviour
{
    public string need;

    void OnMouseDown()
    {
        foreach (Transform child in Inv.panel)
        {
            if (child.name == need)
            {
                Inv.Remove(need);
                break;
            }
        }
    }
}
