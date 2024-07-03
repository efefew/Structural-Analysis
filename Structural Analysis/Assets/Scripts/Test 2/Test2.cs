using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class Test2 : MonoBehaviour
{
    private enum Borders
    {
        None, Up, Down, Right, Left, LeftUp, RightUp, LeftDown, RightDown, Position
    }
    [Min(0)]
    public float minHeight, minWidth;
    [SerializeField]
    private GameObject left, right, up, down;
    [SerializeField]
    private GameObject leftUp, rightUp, leftDown, rightDown;
    [SerializeField]
    private GameObject position;
    private Borders directions;
    private RectTransform rectTr;
    private Vector2 startPositionRectTr, startPositionMouse, startScaleRectTr;
    private bool click;
    private void Awake()
    {
        directions = Borders.None;
        rectTr = GetComponent<RectTransform>();
    }

    private void Update()
    {
        CheckClickBorders();
        ChangePositionBorders();
    }

    private void ChangePositionBorders()
    {
        if (!click)
            return;
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            directions = Borders.None;
            click = false;
        }
        else
        {
            switch (directions)
            {
                case Borders.Up:
                    ChangeUpBorder();

                    break;
                case Borders.Down:
                    ChangeDownBorder();
                    break;
                case Borders.Right:
                    ChangeRightBorder();
                    break;
                case Borders.Left:
                    ChangeLeftBorder();
                    break;
                case Borders.LeftUp:
                    ChangeLeftBorder();
                    ChangeUpBorder();
                    break;
                case Borders.RightUp:
                    ChangeRightBorder();
                    ChangeUpBorder();
                    break;
                case Borders.LeftDown:
                    ChangeLeftBorder();
                    ChangeDownBorder();
                    break;
                case Borders.RightDown:
                    ChangeRightBorder();
                    ChangeDownBorder();
                    break;
                case Borders.Position:
                    ChangePositionBorder();
                    break;
                default:
                    break;
            }
        }
    }

    private void CheckClickBorders()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (UnityEngine.EventSystems.RaycastResult el in UnityExtensions.GetUIOnPoint())
            {
                GameObject objectUI = el.gameObject;
                directions = Borders.None;
                if (objectUI == up)
                    directions = Borders.Up;
                else if (objectUI == down)
                    directions = Borders.Down;
                else if (objectUI == left)
                    directions = Borders.Left;
                else if (objectUI == right)
                    directions = Borders.Right;
                else if (objectUI == leftUp)
                    directions = Borders.LeftUp;
                else if (objectUI == rightUp)
                    directions = Borders.RightUp;
                else if (objectUI == leftDown)
                    directions = Borders.LeftDown;
                else if (objectUI == rightDown)
                    directions = Borders.RightDown;
                else if (objectUI == position)
                    directions = Borders.Position;
                if (directions != Borders.None)
                {
                    click = true;
                    startPositionRectTr = new Vector2(rectTr.anchoredPosition.x, rectTr.anchoredPosition.y);
                    startScaleRectTr = new Vector2(rectTr.sizeDelta.x, rectTr.sizeDelta.y);
                    startPositionMouse = Input.mousePosition;
                }
            }
        }
    }
    private void ChangePositionBorder()
    {
        Vector2 deltaMouse = (Vector2)Input.mousePosition - startPositionMouse;
        rectTr.anchoredPosition = startPositionRectTr + deltaMouse;
    }
    private void ChangeUpBorder()
    {
        float deltaMouse = (Input.mousePosition.y - startPositionMouse.y) / 2;
        deltaMouse = startScaleRectTr.y + (deltaMouse * 2) < minHeight ? (minHeight - startScaleRectTr.y) / 2 : deltaMouse;
        rectTr.anchoredPosition = new Vector2(rectTr.anchoredPosition.x, startPositionRectTr.y + deltaMouse);
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, startScaleRectTr.y + (deltaMouse * 2));
    }

    private void ChangeDownBorder()
    {
        float deltaMouse = (Input.mousePosition.y - startPositionMouse.y) / 2;
        deltaMouse = startScaleRectTr.y - (deltaMouse * 2) < minHeight ? -(minHeight - startScaleRectTr.y) / 2 : deltaMouse;
        rectTr.anchoredPosition = new Vector2(rectTr.anchoredPosition.x, startPositionRectTr.y + deltaMouse);
        rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, startScaleRectTr.y - (deltaMouse * 2));
    }
    private void ChangeLeftBorder()
    {
        float deltaMouse = (Input.mousePosition.x - startPositionMouse.x) / 2;
        deltaMouse = startScaleRectTr.x - (deltaMouse * 2) < minWidth ? -(minWidth - startScaleRectTr.x) / 2 : deltaMouse;
        rectTr.anchoredPosition = new Vector2(startPositionRectTr.x + deltaMouse, rectTr.anchoredPosition.y);
        rectTr.sizeDelta = new Vector2(startScaleRectTr.x - (deltaMouse * 2), rectTr.sizeDelta.y);
    }
    private void ChangeRightBorder()
    {
        float deltaMouse = (Input.mousePosition.x - startPositionMouse.x) / 2;
        deltaMouse = startScaleRectTr.x + (deltaMouse * 2) < minWidth ? (minWidth - startScaleRectTr.x) / 2 : deltaMouse;
        rectTr.anchoredPosition = new Vector2(startPositionRectTr.x + deltaMouse, rectTr.anchoredPosition.y);
        rectTr.sizeDelta = new Vector2(startScaleRectTr.x + (deltaMouse * 2), rectTr.sizeDelta.y);
    }
}