using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    public Action OnChangePosition;
    public int id;
    public Transform tr { get; private set; }
    public List<ElementConnection> connections = new();
    public Text text;
    private void Awake() => tr = transform;
    public void ChangePosition(Vector3 position)
    {
        tr.position = position;
        OnChangePosition?.Invoke();
    }

    public bool DuplicateConnectingElement(Element element)
    {
        for (int id = 0; id < connections.Count; id++)
        {
            if (connections[id].fromElement == element)
                return true;
        }

        return false;
    }
    public void Delete()
    {
        for (int id = connections.Count - 1; id >= 0; id--)
            connections[id].Delete();

        Destroy(gameObject);
    }
}
