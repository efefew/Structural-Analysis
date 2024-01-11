using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class Test2 : MonoBehaviour
{
    [SerializeField]
    private GameObject left, right, up, down;
    private RectTransform rectTr;
    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            foreach (var el in UnityExtensions.GetUIOnPoint())
            {
                GameObject objectUI = el.gameObject;
                if (objectUI == up)
                    ChangeUpBorder();
                if (objectUI == down)
                    ChangeDownBorder();
                if (objectUI == left)
                    ChangeLeftBorder();
                if (objectUI == right)
                    ChangeRightBorder();
            }
        }
    }
    private void ChangeUpBorder()
    {
        //up = Input.mousePosition;
    }
    private void ChangeDownBorder()
    {

    }
    private void ChangeLeftBorder()
    {

    }
    private void ChangeRightBorder()
    {

    }
}
