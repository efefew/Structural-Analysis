using UnityEngine;

public class LineArrow : MonoBehaviour
{
    [SerializeField]
    private Element�onnection �onnection;
    private LineRenderer lineRenderer;
    private Transform tr;
    public float offset;
    private void Awake()
    {
        tr = transform;
        lineRenderer = �onnection.curves.line;
        �onnection.OnChangePosition�onnection += ChangePositionArrow;
    }

    private void ChangePositionArrow()
    {
        int count = lineRenderer.positionCount;
        tr.position = lineRenderer.GetPosition(count - 1) - (tr.right * offset);
        float angle = �onnection.toElement
            ? lineRenderer.GetPosition(count - 1).Angle(�onnection.toElement.tr.position)
            : lineRenderer.GetPosition(count - 2).Angle(lineRenderer.GetPosition(count - 1));
        tr.eulerAngles = new Vector3(0, 0, angle);
    }
}
