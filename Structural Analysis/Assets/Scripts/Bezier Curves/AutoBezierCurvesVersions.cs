using UnityEngine;

[RequireComponent(typeof(BezierCurves))]
public class AutoBezierCurvesVersions : MonoBehaviour
{
    public BezierCurves curves { get; protected set; }
    public Element fromElement, toElement;
    private void Awake()
    {
        curves = GetComponent<BezierCurves>();
        curves.points = new Transform[4];
    }

    public virtual void OnChangePositionElement()
    {
        if (!fromElement || !toElement)
            return;
    }
    public virtual void FromElementConnect(Element fromElement)
    {
        fromElement.OnChangePosition += OnChangePositionElement;
        this.fromElement = fromElement;
    }
    public virtual void ToElementConnect(Element toElement)
    {
        toElement.OnChangePosition += OnChangePositionElement;
        this.toElement = toElement;
    }
}
