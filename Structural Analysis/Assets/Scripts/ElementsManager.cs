using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ElementsManager : MonoBehaviour
{
    #region Fields

    private const float DISTANCE_RAY = 1000f;
    private Vector3 pos, startPos, ScreenPos;
    private Element elementMove;
    private Element—onnection connectionMove;

    [SerializeField]
    private Element elementPrefab;

    [SerializeField]
    private Element—onnection connectionPrefab;

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
            CreateElement—onnection(KeyCode.Mouse1);
        }
    }

    private void CreateElement(KeyCode key)
    {
        if (!Input.GetKeyDown(key))
            return;
        elements.Add(Instantiate(elementPrefab, pos.Z(0), Quaternion.identity, transform));
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

    private void CreateElement—onnection(KeyCode key)
    {
        Element element = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));

        if (element)
        {
            if (!connectionMove)
            {
                Element—onnection Òonnection = Instantiate(connectionPrefab, pos, Quaternion.identity, transform);
                element.connections.Add(Òonnection);
                Òonnection.FromElementConnect(element);
                connectionMove = Òonnection;
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

    [ContextMenu("GetOrderCalculation")]
    public void GetOrderCalculation() => Debug.Log(new StructuralAnalysisCTS().StructuralAnalysisChemicalTechnologicalSystems(CreateAdjacencyMatrix()));
    private double[,] CreateAdjacencyMatrix()
    {
        double[,] matrix = new double[elements.Count, elements.Count];
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
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
                    matrix[idElement, id] = 1.0;
                }
            }
        }

        return matrix;
    }
    #endregion Methods
}