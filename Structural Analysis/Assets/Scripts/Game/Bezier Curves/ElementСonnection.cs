using System;

using UnityEngine;

public class ElementConnection : AutoBezierCurvesVersions
{
    public Action OnChangePositionConnection;
    private const float SCALE = 0.5f;
    private const int RIGHT_ANGLE = 90;
    public float offsetStart, offsetEnd;
    [Min(1)]
    public int value = 1;
    public override void OnChangePositionElement()
    {
        base.OnChangePositionElement();
        Vector3[] points = new Vector3[4];
        float angle = fromElement.tr.position.Angle(toElement.tr.position);
        float clampAngle = Mathf.Abs(angle);
        clampAngle = clampAngle > RIGHT_ANGLE ? (RIGHT_ANGLE * 2) - clampAngle : clampAngle;

        float yCoeff = fromElement.tr.position.y - toElement.tr.position.y;
        float xCoeff = fromElement.tr.position.x - toElement.tr.position.x;

        if (clampAngle > RIGHT_ANGLE / 2)
        {
            points[0] = fromElement.tr.position.AddY(-(angle > 0 ? offsetStart : -offsetStart));
            points[1] = fromElement.tr.position.AddY(-yCoeff * SCALE);
            points[2] = toElement.tr.position.AddY(yCoeff * SCALE);
            points[3] = toElement.tr.position.AddY(angle > 0 ? offsetEnd : -offsetEnd);
        }
        else
        {
            points[0] = fromElement.tr.position.AddX(-(angle is < 90 and > -90 ? offsetStart : -offsetStart));
            points[1] = fromElement.tr.position.AddX(-xCoeff * SCALE);
            points[2] = toElement.tr.position.AddX(xCoeff * SCALE);
            points[3] = toElement.tr.position.AddX(angle is < 90 and > -90 ? offsetEnd : -offsetEnd);
        }

        curves.ChangeLine(points);
        OnChangePositionConnection?.Invoke();
    }
    public void OnChangePositionCursor(Vector3 pointCursor)
    {
        base.OnChangePositionElement();
        Vector3[] points = new Vector3[4];
        float angle = Mathf.Abs(fromElement.tr.position.Angle(pointCursor));
        angle = angle > RIGHT_ANGLE ? (RIGHT_ANGLE * 2) - angle : angle;
        float yCoeff = fromElement.tr.position.y - pointCursor.y;
        float xCoeff = fromElement.tr.position.x - pointCursor.x;
        if (angle > RIGHT_ANGLE / 2)
        {
            points[0] = fromElement.tr.position;
            points[1] = fromElement.tr.position.AddY(-yCoeff * SCALE);
            points[2] = pointCursor.AddY(yCoeff * SCALE);
            points[3] = pointCursor;
        }
        else
        {
            points[0] = fromElement.tr.position;
            points[1] = fromElement.tr.position.AddX(-xCoeff * SCALE);
            points[2] = pointCursor.AddX(xCoeff * SCALE);
            points[3] = pointCursor;
        }

        curves.ChangeLine(points);
        OnChangePositionConnection?.Invoke();
    }
    public void Delete()
    {
        if (fromElement)
        {
            fromElement.OnChangePosition -= OnChangePositionElement;
            _ = fromElement.connections.Remove(this);
        }

        if (toElement)
        {
            toElement.OnChangePosition -= OnChangePositionElement;
            _ = toElement.connections.Remove(this);
        }

        Destroy(gameObject);
    }
    public override void ToElementConnect(Element element)
    {
        base.ToElementConnect(element);
        OnChangePositionElement();
    }
}
