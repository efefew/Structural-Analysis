using UnityEngine;

public class LineArrow : MonoBehaviour
{
    [SerializeField]
    private ElementConnection connection;
    private LineRenderer lineRenderer;
    private Transform tr;
    public float offset;
    private void Awake()
    {
        tr = transform;
        lineRenderer = connection.curves.line;
        connection.OnChangePositionConnection += ChangePositionArrow;
    }

    private void ChangePositionArrow()
    {
        int count = lineRenderer.positionCount;
        tr.position = lineRenderer.GetPosition(count - 1) - (tr.right * offset);
        float angle = connection.toElement
            ? lineRenderer.GetPosition(count - 1).Angle(connection.toElement.tr.position)
            : lineRenderer.GetPosition(count - 2).Angle(lineRenderer.GetPosition(count - 1));
        tr.eulerAngles = new Vector3(0, 0, angle);
    }
}
