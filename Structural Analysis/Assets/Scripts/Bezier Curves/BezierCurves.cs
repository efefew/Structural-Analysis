using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class BezierCurves : MonoBehaviour
{
    public Transform[] points;
    private float currentValueBezier;
    private int countPoints = 25;
    public LineRenderer line { get; private set; }
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = countPoints;
    }

    /// <summary>
    /// ��������� ����� � ����������� �����
    /// </summary>
    public void ChangeLine()
    {
        for (int id = 0; id <= countPoints - 1; id++)
            line.SetPosition(id, OnChangeValueBezier(id / (float)(countPoints - 1)));
    }
    /// <summary>
    /// ��������� ����� � ����������� �����
    /// </summary>
    /// <param name="points">�������� ����������� ����� �����</param>
    public void ChangeLine(Vector3[] points)
    {
        for (int id = 0; id <= countPoints - 1; id++)
            line.SetPosition(id, OnChangeValueBezier(points, id / (float)(countPoints - 1)));
    }
    /// <summary>
    /// ������ �����
    /// </summary>
    /// <param name="valueBezier">�������� �����</param>
    /// <returns>����������� ����� �����</returns>
    public Vector3 OnChangeValueBezier(float valueBezier)
    {
        valueBezier = Mathf.Clamp(valueBezier, 0f, 1f);
        int countLayers = points.Length;
        Vector3[][] controlPoints = new Vector3[countLayers][];//1- ������ ����, 2 - ������ �����
        for (int i = 0; i < points.Length; i++)
            controlPoints[i] = new Vector3[points.Length - i];

        for (int idPoint = 0; idPoint < countLayers; idPoint++)
            controlPoints[0][idPoint] = points[idPoint].position;
        for (int idLayer = 1; idLayer < countLayers; idLayer++)
        {
            for (int idPoint = 0; idPoint < controlPoints[idLayer].Length; idPoint++)
                controlPoints[idLayer][idPoint] = Vector3.Lerp(controlPoints[idLayer - 1][idPoint], controlPoints[idLayer - 1][idPoint + 1], valueBezier);
        }

        currentValueBezier = valueBezier;
        return controlPoints[countLayers - 1][0];
    }
    /// <summary>
    /// ������ �����
    /// </summary>
    /// <param name="points">�������� ����������� ����� �����</param>
    /// <param name="valueBezier">�������� �����</param>
    /// <returns>����������� ����� �����</returns>
    public Vector3 OnChangeValueBezier(Vector3[] points, float valueBezier)
    {
        if (valueBezier == -1)
            valueBezier = currentValueBezier;
        valueBezier = Mathf.Clamp(valueBezier, 0f, 1f);
        int countLayers = points.Length;
        Vector3[][] controlPoints = new Vector3[countLayers][];//1- ������ ����, 2 - ������ �����
        for (int i = 0; i < points.Length; i++)
            controlPoints[i] = new Vector3[points.Length - i];

        for (int idPoint = 0; idPoint < countLayers; idPoint++)
            controlPoints[0][idPoint] = points[idPoint];
        for (int idLayer = 1; idLayer < countLayers; idLayer++)
        {
            for (int idPoint = 0; idPoint < controlPoints[idLayer].Length; idPoint++)
                controlPoints[idLayer][idPoint] = Vector3.Lerp(controlPoints[idLayer - 1][idPoint], controlPoints[idLayer - 1][idPoint + 1], valueBezier);
        }

        return controlPoints[countLayers - 1][0];
    }
}
