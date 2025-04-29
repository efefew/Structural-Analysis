using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ElementsManager : MonoBehaviour
{
    private const float DISTANCE_RAY = 1000f;
    private Vector3 _pos, _startPos, _screenPos;
    private Element _elementMove;
    private ElementConnection _connectionMove;

    [FormerlySerializedAs("elementPrefab")] [SerializeField]
    private Element _elementPrefab;

    [FormerlySerializedAs("connectionPrefab")] [SerializeField]
    private ElementConnection _connectionPrefab;
    [SerializeField] private TMP_InputField _input;

    private List<Element> _elements = new();
    private int _idName;
    private Camera _c;
    #region Methods

    private void Start()
    {
        _c = Camera.main;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        Vector2 mousePos = new()
        {
            x = e.mousePosition.x,
            y = _c.pixelHeight - e.mousePosition.y
        };
        _screenPos = new Vector3(mousePos.x, mousePos.y, _c.nearClipPlane);
        _pos = _c.ScreenToWorldPoint(_screenPos);
    }

    private Component RayClick(KeyCode key, string layer = "", System.Type typeComponent = null)
    {
        if (!Input.GetKeyDown(key))
            return null;
        Vector3 endRay = _c.transform.forward * DISTANCE_RAY;
        Vector3 startRay = _pos;
        RaycastHit2D[] hits = layer == "" ? 
            Physics2D.RaycastAll(startRay, endRay) :
            Physics2D.RaycastAll(startRay, endRay, LayerMask.GetMask(layer));
        return (from t in hits where t.transform.GetComponent(typeComponent) select t.transform.GetComponent(typeComponent)).FirstOrDefault();
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
        _elements.Add(Instantiate(_elementPrefab, _pos.Z(0), Quaternion.identity, transform));
        _idName++;
        _elements.Last().text.text = _idName.ToString();
    }
    private void CreateElement(Vector2 position)
    {
        _elements.Add(Instantiate(_elementPrefab, position, Quaternion.identity, transform));
        _idName++;
        _elements.Last().text.text = _idName.ToString();
    }
    private void DeleteElement(KeyCode key)
    {
        Element deleteElement = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));

        if (!deleteElement)
            return;
        if (_elements.Contains(deleteElement))
            _ = _elements.Remove(deleteElement);
        deleteElement.Delete();
    }

    private void CreateElementConnection(KeyCode key)
    {
        Element element = (Element)RayClick(key, layer: "Element", typeComponent: typeof(Element));

        if (element)
        {
            if (!_connectionMove)
            {
                ElementConnection connection = Instantiate(_connectionPrefab, _pos, Quaternion.identity, transform);
                element.connections.Add(connection);
                connection.FromElementConnect(element);
                _connectionMove = connection;
            }
            else
            {
                if (element.DuplicateConnectingElement(_connectionMove.fromElement))
                {
                    _connectionMove.Delete();
                    _connectionMove = null;
                    return;
                }

                element.connections.Add(_connectionMove);
                _connectionMove.ToElementConnect(element);
                _connectionMove = null;
            }
        }
        else
        {
            if (!_connectionMove)
                return;
            if (Input.GetKeyDown(key))
            {
                _connectionMove.Delete();
                _connectionMove = null;
                return;
            }
        }

        if (!_connectionMove)
            return;

        _connectionMove.OnChangePositionCursor(_pos.Z(0));
    }
    private void CreateElementConnection(Element from, Element to, int value)
    {
        if (from == to)
            return;

        ElementConnection connection = Instantiate(_connectionPrefab, _pos, Quaternion.identity, transform);

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
        if (element)
        {
            _elementMove = element;
            _startPos = _pos - _elementMove.tr.position;
        }

        if (Input.GetKey(key) && _elementMove)
            _elementMove.ChangePosition(new Vector2(_pos.x - _startPos.x, _pos.y - _startPos.y));
        else
            _elementMove = null;
    }
    private string GetOrderCalculation(int[,] matrix) => new StructuralAnalysisCts().StructuralAnalysisChemicalTechnologicalSystems(matrix);

    public void GetOrderCalculation()
    {
        if(_input.text == "") return;
        string text = GetOrderCalculation(CreateAdjacencyMatrix());
        ExportMatrix(text, _input.text);
    }

    private static void ExportMatrix(string text, string path)
    {
        FileStream f = new($"{path}.xls", FileMode.Create);
        f.Close();
        using StreamWriter writer = new($"{path}.xls");
        writer.Write(text);
    }

    public void CreateScheme()
    {
        if(_input.text == "") return;
        transform.Clear();
        int[,] matrix = TestCTS.ReadMatrix(_input.text);
        int count = matrix.GetLength(0);

        CreateElements(count);
        CreateElementConnections(matrix);

    }
    private void CreateElements(int count)
    {
        const float RADIUS = 10f;
        Transform point = new GameObject().transform;
        for (int id = 0; id < count; id++)
        {
            point.eulerAngles = Vector3.zero.Z(id * 360f / count);
            point.localPosition = Vector3.zero + (point.right * RADIUS * count);
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
                    CreateElementConnection(_elements[y], _elements[x], matrix[x, y]);
            }
        }
    }
    private int[,] CreateAdjacencyMatrix()
    {
        int[,] matrix = new int[_elements.Count, _elements.Count];
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
                matrix[x, y] = 0;
        }

        for (int idElement = 0; idElement < _elements.Count; idElement++)
        {
            for (int idConnection = 0; idConnection < _elements[idElement].connections.Count; idConnection++)
            {
                if (_elements[idElement].connections[idConnection].fromElement != _elements[idElement]) continue;
                int id = _elements.IndexOf(_elements[idElement].connections[idConnection].toElement);
                matrix[idElement, id] = _elements[idElement].connections[idConnection].value;
            }
        }

        return matrix;
    }
    #endregion Methods
}