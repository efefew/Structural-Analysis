using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

public class ElementsManager : MonoBehaviour
{
    #region Fields

    private const float DISTANCE_RAY = 1000f;
    private const float ELEMENT_OFFSET = 25f;
    private Vector3 pos, startPos, ScreenPos;
    private Element elementMove;
    private ElementConnection connectionMove;

    [SerializeField]
    private Element elementPrefab;

    [SerializeField]
    private ElementConnection connectionPrefab;

    private List<Element> elements = new();
    private int idName = 0;

    #endregion Fields

    #region Methods

    private void OnGUI()
    {
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new()
        {
            x = e.mousePosition.x,
            y = c.pixelHeight - e.mousePosition.y
        };
        ScreenPos = new Vector3(mousePos.x, mousePos.y, c.nearClipPlane);
        pos = c.ScreenToWorldPoint(ScreenPos);
    }

    private Component RayClick(KeyCode key, string layer = "", System.Type typeComponent = null)
    {
        if (!Input.GetKeyDown(key))
            return null;
        Vector3 EndRay = Camera.main.transform.forward * DISTANCE_RAY;
        Vector3 StartRay = pos;
        RaycastHit2D[] hits = layer == "" ? Physics2D.RaycastAll(StartRay, EndRay) : Physics2D.RaycastAll(StartRay, EndRay, LayerMask.GetMask(layer));
        for (int id = 0; id < hits.Length; id++)
        {
            if (hits[id].transform.GetComponent(typeComponent))
                return hits[id].transform.GetComponent(typeComponent);
        }

        return null;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            CreateElement(KeyCode.Mouse0);
            DeleteElement(KeyCode.Mouse1);
        }
        else
        {
            MoveElement(KeyCode.Mouse0);
            CreateElementConnection(KeyCode.Mouse1);
        }
    }

    private void CreateElement(KeyCode key)
    {
        if (!Input.GetKeyDown(key))
            return;
        CreateElement();
    }
    private void CreateElement()
    {
        elements.Add(Instantiate(elementPrefab, pos.Z(0), Quaternion.identity, transform));
        idName++;
        elements.Last().text.text = idName.ToString();
    }
    private void CreateElement(Vector2 position)
    {
        elements.Add(Instantiate(elementPrefab, position, Quaternion.identity, transform));
        idName++;
        elements.Last().text.text = idName.ToString();
    }
    private void DeleteElement(KeyCode key)
    {
        Element deleteElement = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));

        if (deleteElement == null)
            return;
        if (elements.Contains(deleteElement))
            _ = elements.Remove(deleteElement);
        deleteElement.Delete();
    }

    private void CreateElementConnection(KeyCode key)
    {
        Element element = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));

        if (element)
        {
            if (!connectionMove)
            {
                ElementConnection connection = Instantiate(connectionPrefab, pos, Quaternion.identity, transform);
                element.connections.Add(connection);
                connection.FromElementConnect(element);
                connectionMove = connection;
            }
            else
            {
                if (element.DuplicateConnectingElement(connectionMove.fromElement))
                {
                    connectionMove.Delete();
                    connectionMove = null;
                    return;
                }

                element.connections.Add(connectionMove);
                connectionMove.ToElementConnect(element);
                connectionMove = null;
            }
        }
        else
        {
            if (!connectionMove)
                return;
            if (Input.GetKeyDown(key))
            {
                connectionMove.Delete();
                connectionMove = null;
                return;
            }
        }

        if (!connectionMove)
            return;

        connectionMove.OnChangePositionCursor(pos.Z(0));
    }
    private void CreateElementConnection(Element from, Element to, int value)
    {
        if (from == to)
            return;

        ElementConnection connection = Instantiate(connectionPrefab, pos, Quaternion.identity, transform);

        from.connections.Add(connection);
        connection.FromElementConnect(from);

        to.connections.Add(connection);
        connection.ToElementConnect(to);

        connection.value = value;
        connection.OnChangePositionElement();
    }
    private void MoveElement(KeyCode key)
    {
        Element element = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));
        if (element != null)
        {
            elementMove = element;
            startPos = pos - elementMove.tr.position;
        }

        if (Input.GetKey(key) && elementMove)
            elementMove.ChangePosition(new Vector2(pos.x - startPos.x, pos.y - startPos.y));
        else
            elementMove = null;
    }

    public void GetOrderCalculation() => Debug.Log(new StructuralAnalysisCTS().StructuralAnalysisChemicalTechnologicalSystems(CreateAdjacencyMatrix()));
    public void CreateScheme()
    {
        using StreamReader reader = new("input.txt");
        int count = reader.ReadLine().ToInt();
        int y = 0;
        int[,] matrix = new int[count, count];
        while (!reader.EndOfStream)
        {
            string[] line = reader.ReadLine().Split('\t');
            for (int x = 0; x < count; x++)
                matrix[x, y] = line[x].ToInt();

            y++;
        }

        CreateElements(count);
        CreateElementConnections(matrix);

    }
    private void CreateElements(int count)
    {
        float radius = 10f;
        Transform point = new GameObject().transform;
        for (int id = 0; id < count; id++)
        {
            point.eulerAngles = Vector3.zero.Z(id * 360f / count);
            point.localPosition = Vector3.zero + (point.right * radius * count);
            CreateElement(point.position);
        }

        Destroy(point.gameObject);
    }
    private void CreateElementConnections(int[,] matrix)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                if (matrix[x, y] != 0)
                    CreateElementConnection(elements[y], elements[x], matrix[x, y]);
            }
        }
    }
    private int[,] CreateAdjacencyMatrix()
    {
        int[,] matrix = new int[elements.Count, elements.Count];
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
                matrix[x, y] = 0;
        }

        int id;
        for (int idElement = 0; idElement < elements.Count; idElement++)
        {
            for (int idConnection = 0; idConnection < elements[idElement].connections.Count; idConnection++)
            {
                if (elements[idElement].connections[idConnection].fromElement == elements[idElement])
                {
                    id = elements.IndexOf(elements[idElement].connections[idConnection].toElement);
                    matrix[idElement, id] = elements[idElement].connections[idConnection].value;
                }
            }
        }

        return matrix;
    }
    #endregion Methods
}